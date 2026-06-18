using System;
using System.Collections;
using UnityEngine;

namespace FinalRogue
{
    public class WeaponController : MonoBehaviour
    {
        const float ReloadDuration = 0.5f;
        const float LaserVisibleDuration = 0.1f;

        [SerializeField] Transform firePoint;
        [SerializeField] GameInput gameInput;
        [SerializeField] PlayerAim playerAim;
        [SerializeField] LayerMask raycastMask = ~0;
        [SerializeField] Color laserColor = new(1f, 0.25f, 0.25f, 0.9f);
        [SerializeField] float laserWidth = 0.06f;

        WeaponData currentWeapon;
        Transform ownerRoot;
        float fireCooldown;
        int currentAmmo;
        float reloadTimer;
        bool isReloading;
        LineRenderer laserLine;
        Coroutine hideLaserRoutine;

        public WeaponData CurrentWeapon => currentWeapon;
        public int CurrentAmmo => currentAmmo;
        public int MagazineSize => currentWeapon != null ? currentWeapon.EffectiveMagazineSize : 0;
        public bool IsReloading => isReloading;

        public event Action AmmoChanged;
        public event Action<WeaponData> WeaponEquipped;

        void Awake()
        {
            ownerRoot = transform;
            if (gameInput == null)
                gameInput = GetComponent<GameInput>();
            if (playerAim == null)
                playerAim = GetComponent<PlayerAim>();
            if (firePoint == null)
                firePoint = transform;
        }

        public void Configure(Transform fire, GameInput input, PlayerAim aim)
        {
            firePoint = fire != null ? fire : transform;
            gameInput = input;
            playerAim = aim;

            EntitySetupUtility.SetField(this, "firePoint", firePoint);
            EntitySetupUtility.SetField(this, "gameInput", gameInput);
            EntitySetupUtility.SetField(this, "playerAim", playerAim);
        }

        void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
                return;

            if (currentWeapon == null || gameInput == null || playerAim == null)
                return;

            UpdateReload();

            if (isReloading)
                return;

            if (gameInput.ReloadPressed)
            {
                TryStartReload();
                return;
            }

            fireCooldown -= Time.deltaTime;
            if (!gameInput.AttackHeld || fireCooldown > 0f)
                return;

            if (currentAmmo <= 0)
            {
                TryStartReload();
                return;
            }

            if (!TryFire())
                return;

            currentAmmo--;
            NotifyAmmoChanged();
            fireCooldown = 1f / Mathf.Max(0.01f, currentWeapon.fireRate);
        }

        void UpdateReload()
        {
            if (!isReloading)
                return;

            reloadTimer -= Time.deltaTime;
            if (reloadTimer > 0f)
                return;

            isReloading = false;
            currentAmmo = MagazineSize;
            NotifyAmmoChanged();
        }

        void TryStartReload()
        {
            if (isReloading || currentWeapon == null || currentAmmo >= MagazineSize)
                return;

            isReloading = true;
            reloadTimer = ReloadDuration;
            NotifyAmmoChanged();
        }

        public void Equip(WeaponData weapon, int? ammo = null)
        {
            isReloading = false;
            reloadTimer = 0f;
            fireCooldown = 0f;
            currentWeapon = weapon;

            if (weapon == null)
            {
                currentAmmo = 0;
            }
            else
            {
                int magazine = weapon.EffectiveMagazineSize;
                int desiredAmmo = ammo ?? magazine;
                currentAmmo = Mathf.Clamp(desiredAmmo, 0, magazine);
            }

            NotifyAmmoChanged();
            WeaponEquipped?.Invoke(currentWeapon);
        }

        void NotifyAmmoChanged() => AmmoChanged?.Invoke();

        bool TryFire()
        {
            Vector2 direction = playerAim.AimDirection;
            if (direction.sqrMagnitude < 0.001f)
                return false;

            if (currentWeapon.attackType == AttackType.Projectile)
                return FireProjectile(direction);

            return FireRaycast(direction);
        }

        bool FireProjectile(Vector2 direction)
        {
            if (currentWeapon.projectilePrefab == null)
            {
                Debug.LogWarning($"{nameof(WeaponController)}: Projectile prefab missing on {currentWeapon.name}");
                return false;
            }

            var projectile = Instantiate(currentWeapon.projectilePrefab, firePoint.position, Quaternion.identity);
            if (!projectile.TryGetComponent(out Projectile projectileLogic))
                return true;

            projectileLogic.Launch(direction, currentWeapon.projectileSpeed, currentWeapon.damage, DamageTeam.Player);
            return true;
        }

        bool FireRaycast(Vector2 direction)
        {
            Vector2 origin = firePoint.position;
            if (!WeaponHitUtility.TryFindEnemyHit(origin, direction, currentWeapon.range, ownerRoot, raycastMask, out RaycastHit2D hit))
            {
                ShowLaser(origin, origin + direction * currentWeapon.range);
                return true;
            }

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable == null)
                damageable = hit.collider.GetComponentInParent<IDamageable>();

            damageable?.TakeDamage(currentWeapon.damage);

            ShowLaser(origin, hit.point);
            return true;
        }

        void ShowLaser(Vector2 start, Vector2 end)
        {
            EnsureLaserLine();
            if (laserLine == null)
                return;

            laserLine.enabled = true;
            laserLine.SetPosition(0, new Vector3(start.x, start.y, 0f));
            laserLine.SetPosition(1, new Vector3(end.x, end.y, 0f));

            if (hideLaserRoutine != null)
                StopCoroutine(hideLaserRoutine);

            hideLaserRoutine = StartCoroutine(HideLaserAfterDelay());
        }

        void EnsureLaserLine()
        {
            if (laserLine != null)
                return;

            var lineObject = new GameObject("WeaponLaserLine");
            lineObject.transform.SetParent(firePoint, false);
            laserLine = lineObject.AddComponent<LineRenderer>();
            laserLine.useWorldSpace = true;
            laserLine.positionCount = 2;
            laserLine.startWidth = laserWidth;
            laserLine.endWidth = laserWidth;
            laserLine.numCapVertices = 2;
            laserLine.sortingOrder = 20;

            Shader shader = Shader.Find("Sprites/Default")
                ?? Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader != null)
                laserLine.material = new Material(shader);

            laserLine.startColor = laserColor;
            laserLine.endColor = laserColor;
            laserLine.enabled = false;
        }

        IEnumerator HideLaserAfterDelay()
        {
            yield return new WaitForSeconds(LaserVisibleDuration);
            if (laserLine != null)
                laserLine.enabled = false;
            hideLaserRoutine = null;
        }
    }
}