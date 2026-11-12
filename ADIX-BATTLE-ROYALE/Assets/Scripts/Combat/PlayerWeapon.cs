using System;
using System.Collections;
using UnityEngine;

/// <summary>Attach to a weapon transform; assign a camera or muzzle origin.</summary>
public sealed class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform muzzle;
    [SerializeField, Min(1f)] private float damage = 24f;
    [SerializeField, Min(1f)] private float range = 180f;
    [SerializeField, Min(0.05f)] private float shotsPerSecond = 6f;
    [SerializeField, Min(1)] private int magazineSize = 30;
    [SerializeField, Min(0f)] private float reloadSeconds = 1.7f;
    [SerializeField] private LayerMask hitMask = ~0;

    public int Ammo { get; private set; }
    public int ReserveAmmo { get; private set; } = 120;
    public bool IsReloading { get; private set; }
    public event Action<int, int> AmmoChanged;
    public event Action<Vector3> HitConfirmed;
    private float nextShotTime;

    private void Awake()
    {
        Ammo = magazineSize;
        if (aimCamera == null) aimCamera = Camera.main;
        AmmoChanged?.Invoke(Ammo, ReserveAmmo);
    }

    public void Fire()
    {
        if (IsReloading || Time.time < nextShotTime || Ammo <= 0 || aimCamera == null) return;
        nextShotTime = Time.time + 1f / shotsPerSecond;
        Ammo--;
        AmmoChanged?.Invoke(Ammo, ReserveAmmo);

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            var target = hit.collider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage);
            HitConfirmed?.Invoke(hit.point);
        }
    }

    public void Reload()
    {
        if (!IsReloading && Ammo < magazineSize && ReserveAmmo > 0)
            StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        yield return new WaitForSeconds(reloadSeconds);
        int needed = magazineSize - Ammo;
        int loaded = Mathf.Min(needed, ReserveAmmo);
        Ammo += loaded;
        ReserveAmmo -= loaded;
        IsReloading = false;
        AmmoChanged?.Invoke(Ammo, ReserveAmmo);
    }

    public void AddReserveAmmo(int amount) => ReserveAmmo = Mathf.Max(0, ReserveAmmo + amount);
}