using System.Collections;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public KeyCode reloadKeyCode = KeyCode.R;
    public WeaponStyle mWeaponStyle;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject enemyHpBar;
    public bool isReloading = false;

    public Sprite passiveIcon;
    public Sprite activeIcon;

    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;
    public float fireRate = 15f;
    public float reloadTime = 1f;

    public int maxAmmo = 10;
    public int magazineSize = 400;
    public int currentAmmo;

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

        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= _nextTimeToFire)
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