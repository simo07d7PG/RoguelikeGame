using UnityEngine;
using UnityEngine.UI;

namespace FinalRogue
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] Text healthText;
        [SerializeField] Text coinText;
        [SerializeField] Text stageText;
        [SerializeField] Text weaponText;
        [SerializeField] Text ammoText;
        [SerializeField] Text playTimeText;

        [Header("Panels")]
        [SerializeField] GameObject hudPanel;
        [SerializeField] GameObject stageClearPanel;
        [SerializeField] GameObject shopPanel;
        [SerializeField] GameObject gameOverPanel;

        [Header("Stage Clear")]
        [SerializeField] Text stageClearText;
        [SerializeField] Button stageClearContinueButton;

        [Header("Shop")]
        [SerializeField] Button healButton;
        [SerializeField] Text healButtonText;
        [SerializeField] Button[] weaponButtons;
        [SerializeField] Text[] weaponButtonTexts;
        [SerializeField] Button shopContinueButton;
        [SerializeField] Text shopCoinText;

        [Header("Game Over")]
        [SerializeField] InputField nicknameInput;
        [SerializeField] Text gameOverText;
        [SerializeField] Button restartButton;
        [SerializeField] Text leaderboardText;

        GameManager gameManager;
        ShopManager shopManager;
        PlayerHealth playerHealth;
        WeaponController weaponController;

        public void AutoWire()
        {
            Canvas canvas = EntitySetupUtility.FindFirst<Canvas>();
            if (canvas == null)
                return;

            Transform root = canvas.transform;
            hudPanel = UIHierarchyUtility.FindObject(root, "HUD");
            stageClearPanel = UIHierarchyUtility.FindObject(root, "StageClear");
            shopPanel = UIHierarchyUtility.FindObject(root, "Shop");
            gameOverPanel = UIHierarchyUtility.FindObject(root, "GameOver");

            healthText = UIHierarchyUtility.FindComponent<Text>(root, "HealthText");
            coinText = UIHierarchyUtility.FindComponent<Text>(root, "CoinText");
            stageText = UIHierarchyUtility.FindComponent<Text>(root, "StageText");
            weaponText = UIHierarchyUtility.FindComponent<Text>(root, "WeaponText");
            ammoText = UIHierarchyUtility.FindComponent<Text>(root, "AmmoText");
            playTimeText = UIHierarchyUtility.FindComponent<Text>(root, "PlayTimeText");
            stageClearText = UIHierarchyUtility.FindComponent<Text>(root, "StageClearText");
            healButtonText = UIHierarchyUtility.FindComponent<Text>(root, "HealButtonText");
            shopCoinText = UIHierarchyUtility.FindComponent<Text>(root, "ShopCoinText");
            gameOverText = UIHierarchyUtility.FindComponent<Text>(root, "GameOverText");
            leaderboardText = UIHierarchyUtility.FindComponent<Text>(root, "LeaderboardText");
            nicknameInput = UIHierarchyUtility.FindComponent<InputField>(root, "NicknameInput");

            stageClearContinueButton = UIHierarchyUtility.FindComponent<Button>(root, "StageClearContinueButton");
            healButton = UIHierarchyUtility.FindComponent<Button>(root, "HealButton");
            shopContinueButton = UIHierarchyUtility.FindComponent<Button>(root, "ShopContinueButton");
            restartButton = UIHierarchyUtility.FindComponent<Button>(root, "RestartButton");

            weaponButtons = new[]
            {
                UIHierarchyUtility.FindComponent<Button>(root, "WeaponButton0"),
                UIHierarchyUtility.FindComponent<Button>(root, "WeaponButton1"),
                UIHierarchyUtility.FindComponent<Button>(root, "WeaponButton2")
            };

            weaponButtonTexts = new[]
            {
                UIHierarchyUtility.FindComponent<Text>(root, "WeaponButtonText0"),
                UIHierarchyUtility.FindComponent<Text>(root, "WeaponButtonText1"),
                UIHierarchyUtility.FindComponent<Text>(root, "WeaponButtonText2")
            };

            PersistFields();
        }

        public void EnsureRuntimeUI()
        {
            bool rebuilt = RuntimeUIBuilder.BuildIfMissing();
            if (rebuilt || hudPanel == null)
                AutoWire();
        }

        public void BindListeners()
        {
            if (stageClearContinueButton != null)
            {
                stageClearContinueButton.onClick.RemoveListener(OnStageClearContinue);
                stageClearContinueButton.onClick.AddListener(OnStageClearContinue);
            }

            if (shopContinueButton != null)
            {
                shopContinueButton.onClick.RemoveListener(OnShopContinue);
                shopContinueButton.onClick.AddListener(OnShopContinue);
            }

            if (healButton != null)
            {
                healButton.onClick.RemoveListener(OnHealClicked);
                healButton.onClick.AddListener(OnHealClicked);
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(OnRestartClicked);
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (weaponButtons == null)
                return;

            for (int i = 0; i < weaponButtons.Length; i++)
            {
                int index = i;
                if (weaponButtons[i] == null)
                    continue;

                weaponButtons[i].onClick.RemoveAllListeners();
                weaponButtons[i].onClick.AddListener(() => OnWeaponClicked(index));
            }
        }

        void Awake()
        {
            EnsureManagers();
            BindListeners();
        }

        void Start()
        {
            EnsureManagers();

            if (gameManager != null)
            {
                gameManager.CoinsChanged += RefreshCoins;
                gameManager.StateChanged += OnGameStateChanged;
                if (gameManager.RunData != null)
                    BindPlayerRefs();
                OnGameStateChanged(gameManager.State);
            }
            else
            {
                ShowHud();
            }
        }

        void Update()
        {
            if (gameManager == null || gameManager.State != GameState.Playing)
                return;

            if (playTimeText != null)
                playTimeText.text = GameFormatUtility.FormatTime(gameManager.PlayTime);
        }

        void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.CoinsChanged -= RefreshCoins;
                gameManager.StateChanged -= OnGameStateChanged;
            }

            if (weaponController != null)
            {
                weaponController.AmmoChanged -= RefreshAmmo;
                weaponController.WeaponEquipped -= OnWeaponEquipped;
            }
        }

        void OnWeaponEquipped(WeaponData weapon)
        {
            if (weaponText != null)
                weaponText.text = weapon != null ? weapon.weaponName : "-";
            RefreshAmmo();
        }

        void EnsureManagers()
        {
            if (gameManager == null)
                gameManager = GameManager.Instance;

            if (shopManager == null && gameManager != null)
                shopManager = gameManager.GetComponent<ShopManager>();
        }

        void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Playing)
                ShowHud();
        }

        void BindPlayerRefs()
        {
            PlayerEntity playerEntity = EntitySetupUtility.FindFirst<PlayerEntity>();
            PlayerController player = playerEntity != null
                ? playerEntity.GetComponent<PlayerController>()
                : EntitySetupUtility.FindFirst<PlayerController>();
            if (player == null)
                return;

            playerHealth = player.GetComponent<PlayerHealth>();
            weaponController = player.GetComponent<WeaponController>();

            if (playerHealth != null)
                playerHealth.HealthChanged += OnHealthChanged;

            if (weaponController != null)
            {
                weaponController.AmmoChanged -= RefreshAmmo;
                weaponController.AmmoChanged += RefreshAmmo;
                weaponController.WeaponEquipped -= OnWeaponEquipped;
                weaponController.WeaponEquipped += OnWeaponEquipped;
                RefreshAmmo();
            }
        }

        void OnHealthChanged(int current, int max)
        {
            if (healthText != null)
                healthText.text = $"HP {current}/{max}";
        }

        public void RefreshAll(RunPersistentData runData, int stageIndex, float playTime)
        {
            RefreshCoins(runData.Coins);
            if (stageText != null)
                stageText.text = $"Stage {stageIndex + 1}";
            if (weaponText != null)
                weaponText.text = runData.currentWeapon != null ? runData.currentWeapon.weaponName : "-";
            if (playTimeText != null)
                playTimeText.text = GameFormatUtility.FormatTime(playTime);
            if (healthText != null)
                healthText.text = $"HP {runData.currentHealth}/{runData.maxHealth}";

            RefreshAmmo();
        }

        void RefreshAmmo()
        {
            if (ammoText == null)
                return;

            if (weaponController == null)
            {
                ammoText.text = "- / -";
                return;
            }

            if (weaponController.IsReloading)
            {
                ammoText.text = "RELOADING";
                return;
            }

            ammoText.text = $"{weaponController.CurrentAmmo} / {weaponController.MagazineSize}";
        }

        public void RefreshCoins(int coins)
        {
            if (coinText != null)
                coinText.text = $"Coin {coins}";
            if (shopCoinText != null)
                shopCoinText.text = $"Coin {coins}";
        }

        public void ShowStageClear(int clearedStageIndex, RunPersistentData runData)
        {
            SetPanel(stageClearPanel);
            if (stageClearText != null)
                stageClearText.text = $"Stage {clearedStageIndex + 1} Clear!\nLoop {runData.loopCount}";
        }

        public void ShowShop(RunPersistentData runData, ShopManager shop)
        {
            shopManager = shop;
            SetPanel(shopPanel);
            RefreshCoins(runData.Coins);

            if (healButtonText != null && shop != null)
                healButtonText.text = $"Heal +{shop.GetHealAmount()} ({shop.GetHealCost()})";

            var offers = shop != null ? shop.GetWeaponOffers() : null;
            for (int i = 0; i < weaponButtons.Length; i++)
            {
                bool hasOffer = offers != null && i < offers.Length && offers[i] != null;
                if (weaponButtons[i] != null)
                    weaponButtons[i].gameObject.SetActive(hasOffer);

                if (hasOffer && i < weaponButtonTexts.Length && weaponButtonTexts[i] != null)
                {
                    var weapon = offers[i];
                    weaponButtonTexts[i].text = $"{weapon.weaponName} ({weapon.shopPrice})";
                }
            }
        }

        public void ShowGameOver(string nickname, float playTime, SaveManager saveManager)
        {
            SetPanel(gameOverPanel);
            if (gameOverText != null)
                gameOverText.text = $"Game Over\n{GameFormatUtility.FormatTime(playTime)}";
            if (nicknameInput != null && string.IsNullOrEmpty(nicknameInput.text))
                nicknameInput.text = nickname;
            RefreshLeaderboard(saveManager);
        }

        public void RefreshLeaderboard(SaveManager saveManager)
        {
            if (leaderboardText != null)
                leaderboardText.text = GameFormatUtility.BuildLeaderboardText(saveManager);
        }

        public void ShowHud()
        {
            SetPanel(hudPanel);
        }

        void SetPanel(GameObject activePanel)
        {
            if (hudPanel != null)
                hudPanel.SetActive(activePanel == hudPanel);
            if (stageClearPanel != null)
                stageClearPanel.SetActive(activePanel == stageClearPanel);
            if (shopPanel != null)
                shopPanel.SetActive(activePanel == shopPanel);
            if (gameOverPanel != null)
                gameOverPanel.SetActive(activePanel == gameOverPanel);
        }

        void OnStageClearContinue()
        {
            EnsureManagers();
            gameManager?.OpenShop();
        }

        void OnShopContinue()
        {
            EnsureManagers();
            shopManager?.CloseShop();
        }

        void OnHealClicked()
        {
            if (gameManager == null || shopManager == null)
                return;

            if (playerHealth == null)
                BindPlayerRefs();

            if (shopManager.TryBuyHeal(gameManager.RunData, playerHealth))
            {
                RefreshCoins(gameManager.RunData.Coins);
                RefreshAll(gameManager.RunData, StageManager.Instance != null ? StageManager.Instance.CurrentStageIndex : 0, gameManager.PlayTime);
            }
        }

        void OnWeaponClicked(int index)
        {
            if (gameManager == null || shopManager == null)
                return;

            if (weaponController == null)
                BindPlayerRefs();

            var offers = shopManager.GetWeaponOffers();
            if (offers == null || index < 0 || index >= offers.Length)
                return;

            if (shopManager.TryBuyWeapon(gameManager.RunData, weaponController, offers[index]))
            {
                RefreshCoins(gameManager.RunData.Coins);
                RefreshAll(gameManager.RunData, StageManager.Instance != null ? StageManager.Instance.CurrentStageIndex : 0, gameManager.PlayTime);
            }
        }

        void OnRestartClicked()
        {
            if (gameManager == null)
                return;

            string submitted = nicknameInput != null ? nicknameInput.text : string.Empty;
            gameManager.SubmitGameOver(submitted);
            gameManager.RestartRun();
            RefreshLeaderboard(SaveManager.Instance);
            ShowHud();
        }

        void PersistFields()
        {
#if UNITY_EDITOR
            EntitySetupUtility.SetField(this, "healthText", healthText);
            EntitySetupUtility.SetField(this, "coinText", coinText);
            EntitySetupUtility.SetField(this, "stageText", stageText);
            EntitySetupUtility.SetField(this, "weaponText", weaponText);
            EntitySetupUtility.SetField(this, "ammoText", ammoText);
            EntitySetupUtility.SetField(this, "playTimeText", playTimeText);
            EntitySetupUtility.SetField(this, "hudPanel", hudPanel);
            EntitySetupUtility.SetField(this, "stageClearPanel", stageClearPanel);
            EntitySetupUtility.SetField(this, "shopPanel", shopPanel);
            EntitySetupUtility.SetField(this, "gameOverPanel", gameOverPanel);
            EntitySetupUtility.SetField(this, "stageClearText", stageClearText);
            EntitySetupUtility.SetField(this, "stageClearContinueButton", stageClearContinueButton);
            EntitySetupUtility.SetField(this, "healButton", healButton);
            EntitySetupUtility.SetField(this, "healButtonText", healButtonText);
            EntitySetupUtility.SetField(this, "shopContinueButton", shopContinueButton);
            EntitySetupUtility.SetField(this, "shopCoinText", shopCoinText);
            EntitySetupUtility.SetField(this, "nicknameInput", nicknameInput);
            EntitySetupUtility.SetField(this, "gameOverText", gameOverText);
            EntitySetupUtility.SetField(this, "restartButton", restartButton);
            EntitySetupUtility.SetField(this, "leaderboardText", leaderboardText);
            EntitySetupUtility.SetFieldArray(this, "weaponButtons", weaponButtons);
            EntitySetupUtility.SetFieldArray(this, "weaponButtonTexts", weaponButtonTexts);
#endif
        }
    }
}