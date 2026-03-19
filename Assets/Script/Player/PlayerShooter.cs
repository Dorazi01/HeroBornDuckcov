using UnityEngine;
using System.Collections;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AudioClip gunshotSound;

    [SerializeField] private float fireRate = 0.07f;
    [SerializeField] private float reloadTime = 1.5f;

    [SerializeField] private PlayerCamera playerCamera;
    PlayerController player;

    private AudioSource audioSource;

    private float nextFireTime;

    private bool isFireHeld;
    private bool isShooting;
    private bool isSprinting;


    void Awake()
    {
        player = GetComponent<PlayerController>();
        playerCamera = GetComponent<PlayerCamera>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        player.currentAmmo = player.maxAmmo;

    }

    void Update()
    {
        Fire();
        DrawLaser();
    }

    public void SetFireHeld(bool value) => isFireHeld = value;
    public void SetSprinting(bool value) => isSprinting = value;
    public void SetAiming(bool value) => player.isAiming = value;

    private void Fire()
    {
        if (isFireHeld && !isShooting && (!isSprinting || player.isAiming) &&
            player.currentAmmo > 0 && !player.isReloading && Time.time >= nextFireTime)
        {
            isShooting = true;
            nextFireTime = Time.time + fireRate;
            player.currentAmmo--;

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit, 1000f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) { targetPoint = hit.point; }
            else { targetPoint = ray.GetPoint(1000f); }

            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            Vector3 direction = (targetPoint - spawnPos).normalized;

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(direction));

            if (audioSource != null && gunshotSound != null) { audioSource.PlayOneShot(gunshotSound); }

            // 플레이어 충돌 무시
            Collider[] playerColliders = GetComponentsInChildren<Collider>();
            Collider bulletCollider = bullet.GetComponent<Collider>();
            foreach (Collider col in playerColliders) Physics.IgnoreCollision(col, bulletCollider);

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.useGravity = false;
            bulletRb.linearVelocity = direction * 300f;

            // 반동 적용
            float randomRecoilX = Random.Range(playerCamera.recoilKickX, playerCamera.recoilKickX * 1.5f);
            float randomRecoilY = Random.Range(-playerCamera.recoilKickY, playerCamera.recoilKickY);
            if (playerCamera != null)
            {
                playerCamera.ApplyRecoil(randomRecoilX, randomRecoilY);
            }

            isShooting = false;
        }
    }

    void DrawLaser()
    {
        if (player.laserLine == null || firePoint == null) return;

        // 1. 카메라 정중앙에서 목표 찾기
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        // 2. 목표지점 결정 (닿은 곳 or 허공)
        if (Physics.Raycast(ray, out hit, 1000f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        // 3. 라인 렌더러로 선 그리기 (총구 -> 목표)
        player.laserLine.SetPosition(0, firePoint.position); // 시작점: 총구
        player.laserLine.SetPosition(1, targetPoint);        // 끝점: 크로스헤어가 닿은 곳

        player.targetDistance = Vector3.Distance(transform.position, targetPoint);
    }

    public IEnumerator ReloadCoroutine()
    {
        player.isReloading = true;
        player.reloadProgress = 0f;

        float elapsed = 0f;
        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            player.reloadProgress = elapsed / reloadTime;
            yield return null;
        }

        player.reloadProgress = 1f;
        player.currentAmmo = player.maxAmmo;
        player.isReloading = false;
    }

    public bool GetIsReloading() => player.isReloading;
    public int GetCurrentAmmo() => player.currentAmmo;
    public int GetMaxAmmo() => player.maxAmmo;
    public float GetReloadProgress() => player.reloadProgress;
}