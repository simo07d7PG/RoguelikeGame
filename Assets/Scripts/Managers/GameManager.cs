using System;
using UnityEngine;

namespace FinalRogue
{
    public enum GameState
    {
        Playing,
        StageClear,
        Shop,
        GameOver
    }

    [DefaultExecutionOrder(-250)]
    [AddComponentMenu("FinalRogue/Game Manager")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StageManager))]
    [RequireComponent(typeof(ShopManager))]
    [RequireComponent(typeof(UIManager))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] WeaponData startingWeapon;
        [SerializeField] ShopCatalog shopCatalog;
        [SerializeField] GameObject enemyPrefab;
        [SerializeField] StageManager stageManager;
        [SerializeField] ShopManager shopManager;
        [SerializeField] UIManager uiManager;
        [SerializeField] PlayerHealth playerHealth;
        [SerializeField] WeaponController weaponController;
        [SerializeField] Transform player;

        [Header("Run Data")]
        [SerializeField] RunPersistentData runData = new();
        [SerializeField] int startingCoins;
        float playTime;
        int nextStageIndex;
        string nickname = "Player";
        bool initialized;

        public GameState State { get; private set; } = GameState.Playing;
        public RunPersistentData RunData => runData;
        public float PlayTime => playTime;
        public string Nickname => nickname;

        public event Action<GameState> StateChanged;
        public event Action<int> CoinsChanged;

        void Reset() => SetupEntity();

        void Awake()
        {
            SetupEntity();

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            if (initialized)
                return;

            initialized = true;
            BindPlayerComponents();

            if (playerHealth != null)
                playerHealth.Died += OnPlayerDied;

            if (shopManager != null)
                shopManager.ShopClosed += OnShopClosed;

            SetNickname(LobbySessionData.LoadNickname());
            BeginRun();
        }

        void OnDestroy()
        {
            if (playerHealth != null)
                playerHealth.Died -= OnPlayerDied;
            if (shopManager != null)
                shopManager.ShopClosed -= OnShopClosed;
        }

        void Update()
        {
            if (State == GameState.Playing)
                playTime += Time.deltaTime;
        }

        void SetupEntity()
        {
            stageManager = EntitySetupUtility.EnsureComponent<StageManager>(gameObject);
            shopManager = EntitySetupUtility.EnsureComponent<ShopManager>(gameObject);
            SaveManager.EnsureExists();
            uiManager = EntitySetupUtility.EnsureComponent<UIManager>(gameObject);

            if (startingWeapon == null)
                startingWeapon = LoadDefaultAsset<WeaponData>(EntitySetupUtility.DefaultStartingWeaponPath);

            if (shopCatalog == null)
                shopCatalog = LoadDefaultAsset<ShopCatalog>(EntitySetupUtility.DefaultShopCatalogPath);

            if (enemyPrefab == null)
                enemyPrefab = LoadDefaultAsset<GameObject>(EntitySetupUtility.DefaultEnemyPrefabPath);

            BindPlayerComponents();

            StageArea[] stages = FindStages();
            if (stages.Length == 0)
            {
                EnemyData enemyData = LoadDefaultAsset<EnemyData>(EntitySetupUtility.DefaultEnemyDataPath);
                Vector3 stageOrigin = ResolveStageOrigin();
                stages = RuntimeStageFactory.GetOrCreate(enemyData, stageOrigin);
            }

            stageManager.Configure(stages, enemyPrefab, player);
            shopManager.Configure(shopCatalog);
            uiManager.EnsureRuntimeUI();
            uiManager.AutoWire();
            uiManager.BindListeners();
            EntitySetupUtility.EnsureComponent<CameraFollowBootstrap>(gameObject);

            EntitySetupUtility.SetField(this, "startingWeapon", startingWeapon);
            EntitySetupUtility.SetField(this, "shopCatalog", shopCatalog);
            EntitySetupUtility.SetField(this, "enemyPrefab", enemyPrefab);
            EntitySetupUtility.SetField(this, "stageManager", stageManager);
            EntitySetupUtility.SetField(this, "shopManager", shopManager);
            EntitySetupUtility.SetField(this, "uiManager", uiManager);
            EntitySetupUtility.SetField(this, "player", player);
            EntitySetupUtility.SetField(this, "playerHealth", playerHealth);
            EntitySetupUtility.SetField(this, "weaponController", weaponController);
            EntitySetupUtility.SetField(this, "startingCoins", startingCoins);
        }

        void BindPlayerComponents()
        {
            if (player == null)
            {
                PlayerEntity playerEntity = EntitySetupUtility.FindFirst<PlayerEntity>();
                if (playerEntity != null)
                    player = playerEntity.transform;
                else
                {
                    PlayerController playerController = EntitySetupUtility.FindFirst<PlayerController>();
                    if (playerController != null)
                        player = playerController.transform;
                }
            }

            if (player == null)
                return;

            if (playerHealth == null)
                playerHealth = player.GetComponent<PlayerHealth>();
            if (weaponController == null)
                weaponController = player.GetComponent<WeaponController>();

            CameraFollowBootstrap bootstrap = EntitySetupUtility.FindFirst<CameraFollowBootstrap>();
            if (bootstrap == null)
                bootstrap = EntitySetupUtility.EnsureComponent<CameraFollowBootstrap>(gameObject);
            bootstrap.BindPlayer(player);
        }

        Vector3 ResolveStageOrigin()
        {
            if (player != null)
                return player.position;

            Grid grid = EntitySetupUtility.FindFirst<Grid>();
            if (grid != null)
                return grid.transform.position;

            return Vector3.zero;
        }

        static StageArea[] FindStages()
        {
            StageArea[] stages = EntitySetupUtility.FindAll<StageArea>();
            Array.Sort(stages, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
            return stages;
        }

        static T LoadDefaultAsset<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return EntitySetupUtility.LoadAssetAtPath<T>(path);
#else
            return null;
#endif
        }

        void BeginRun()
        {
            playTime = 0f;
            nextStageIndex = 0;
            runData.loopCount = 0;
            runData.Coins = startingCoins;
            runData.maxHealth = playerHealth != null ? playerHealth.MaxHealth : 100;
            runData.currentHealth = runData.maxHealth;
            runData.currentWeapon = startingWeapon;
            runData.currentAmmo = -1;

            ApplyRunDataToPlayer();
            SetState(GameState.Playing);
            stageManager?.LoadStage(nextStageIndex, runData);
            uiManager?.RefreshAll(runData, nextStageIndex, playTime);
        }

        public void SetNickname(string value) => nickname = NicknameUtility.Normalize(value);

        public void EquipWeapon(WeaponData weapon, int? ammo = null)
        {
            if (weapon == null || !weapon.IsValidForCombat())
                return;

            runData.currentWeapon = weapon;
            runData.currentAmmo = ammo ?? -1;

            if (weaponController == null)
                BindPlayerComponents();

            weaponController?.Equip(weapon, ammo);
            uiManager?.RefreshAll(runData, stageManager != null ? stageManager.CurrentStageIndex : 0, playTime);
        }

        public void OnEnemyKilled(int coinReward)
        {
            runData.AddCoins(coinReward);
            CoinsChanged?.Invoke(runData.Coins);
            uiManager?.RefreshCoins(runData.Coins);
        }

        public void OnStageCleared(int clearedStageIndex)
        {
            CaptureRunDataFromPlayer();

            if (clearedStageIndex >= stageManager.StageCount - 1)
            {
                runData.loopCount++;
                nextStageIndex = 0;
            }
            else
            {
                nextStageIndex = clearedStageIndex + 1;
            }

            SetState(GameState.StageClear);
            uiManager?.ShowStageClear(clearedStageIndex, runData);
        }

        public void OpenShop()
        {
            SetState(GameState.Shop);
            uiManager?.ShowShop(runData, shopManager);
        }

        void OnShopClosed()
        {
            ApplyRunDataToPlayer();
            SetState(GameState.Playing);
            stageManager?.LoadStage(nextStageIndex, runData);
            uiManager?.ShowHud();
            uiManager?.RefreshAll(runData, nextStageIndex, playTime);
        }

        void OnPlayerDied()
        {
            SetState(GameState.GameOver);
            uiManager?.ShowGameOver(nickname, playTime, SaveManager.Instance);
        }

        public void SubmitGameOver(string submittedNickname)
        {
            SetNickname(submittedNickname);
            SaveManager.Instance?.SaveLeaderboardEntry(nickname, playTime);
        }

        public void RestartRun()
        {
            BeginRun();
        }

        void SetState(GameState newState)
        {
            State = newState;
            StateChanged?.Invoke(newState);
        }

        void ApplyRunDataToPlayer()
        {
            if (playerHealth != null)
                playerHealth.Initialize(runData.currentHealth, runData.maxHealth);
            if (runData.currentWeapon != null)
                EquipWeapon(runData.currentWeapon, runData.currentAmmo >= 0 ? runData.currentAmmo : null);
        }

        void CaptureRunDataFromPlayer()
        {
            if (playerHealth != null)
            {
                runData.currentHealth = playerHealth.CurrentHealth;
                runData.maxHealth = playerHealth.MaxHealth;
            }
            if (weaponController != null)
            {
                runData.currentWeapon = weaponController.CurrentWeapon;
                runData.currentAmmo = weaponController.CurrentAmmo;
            }
        }
    }
}