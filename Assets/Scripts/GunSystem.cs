using System.Collections;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    // References
    [SerializeField] private KeyCode reloadKeyCode = KeyCode.R;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject enemyHpBar;
    public WeaponStyle mWeaponStyle;
    public Sprite passiveIcon;
    public Sprite activeIcon;

    // Variables
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float impactForce = 30f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private int maxAmmo = 10;
    public bool isReloading = false;
    public int magazineSize = 400;
    public int currentAmmo;

    // Private variables
    private Camera _camera;
    private float _nextTimeToFire = 0f;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (isReloading) return;
        if ((currentAmmo <= 0 || Input.GetKeyDown(reloadKeyCode)) && magazineSize > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= _nextTimeToFire && currentAmmo > 0)
        {
            _nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        if (magazineSize >= maxAmmo)
        {
            magazineSize -= maxAmmo - currentAmmo;
            currentAmmo = maxAmmo;
        }
        else
        {
            currentAmmo = magazineSize;
            magazineSize = 0;
        }

        isReloading = false;
    }

    private void Shoot()
    {
        muzzleFlash.Play();

        currentAmmo--;

        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, range))
        {
            var enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemyHpBar.SetActive(true);
                enemy.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            var impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGo, 2f);
        }
    }
}

public enum WeaponStyle
{
    Primary,
    Secondary,
    Tertiary
}