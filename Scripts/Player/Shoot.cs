using UnityEngine;
using System.Collections;
using CodeMonkey.Utils;

public class Shoot : MonoBehaviour
{
    [Header("Name")]
    [SerializeField] private string weaponName;

    [Header("Aim")]
    [SerializeField] private GameObject barrelEnd;

    [Header("Effects")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject muzzleFlashPrefab;

    [Header("Weapon Stats")]
    [SerializeField] private bool automatic;
    [SerializeField] public float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifeTime = 5f;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo; // Max ammo in one magazine
    [SerializeField] private float reloadTime; // Time to reload
    private int currentAmmo; // Current ammo in the magazine
    private bool isReloading;

    [Header("Screen Shake")]
    [SerializeField] private Cinemachine.NoiseSettings shootShakeProfile;
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeDuration;

    private float timeSinceLastShot;
    private bool isShooting;

    private void Start()
    {
        isShooting = false;
        currentAmmo = maxAmmo; // Start with a full magazine
        isReloading = false;
    }

    private void Update()
    {
        CheckShootingStatus();

        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        timeSinceLastShot += Time.deltaTime;

        if (automatic)
        {
            if (Input.GetMouseButton(0) && timeSinceLastShot >= fireRate)
            {
                ShootBullet(mousePosition);
                ScreenShake.Instance.ShakeCamera(shakeIntensity, shakeDuration, shootShakeProfile);
                timeSinceLastShot = 0f;
                currentAmmo--;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                isShooting = true;
                ScreenShake.Instance.ShakeCamera(shakeIntensity, shakeDuration, shootShakeProfile);
                ShootBullet(mousePosition);
                currentAmmo--;
            }
            else
            {
                isShooting = false;
            }
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reloaded.");
    }

    private void CheckShootingStatus()
    {
        isShooting = Input.GetMouseButton(0);
    }

    private void ShootBullet(Vector3 targetPosition)
    {
        GameObject bullet = Instantiate(bulletPrefab, barrelEnd.transform.position, Quaternion.identity);
        Bullet bulletController = bullet.GetComponent<Bullet>();
        bulletController.InitializeBullet(targetPosition);
        bulletController.SetBulletStats(damage, bulletSpeed, bulletLifeTime);
    }

    //Ammo
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }
}
