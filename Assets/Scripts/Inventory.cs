using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject[] weapons;
    public Image[] itemSlots = new Image[3];
    public GameObject currentWeapon;
    public Text ammoInfo;
    public Sprite nullItem;

    private void Start()
    {
        weapons = new GameObject[3];
    }

    private void Update()
    {
        MyInput();
    }

    private void FixedUpdate()
    {
        if (currentWeapon != null)
        {
            var currentGunSystem = currentWeapon.GetComponent<GunSystem>();
            ammoInfo.text = currentGunSystem.currentAmmo + " / " + currentGunSystem.magazineSize;
        }
        else
        {
            ammoInfo.text = "0 / 0";
        }
    }

    private void MyInput()
    {
        GameObject item = null;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            item = GetItem(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            item = GetItem(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            item = GetItem(2);
        }

        if (item != null)
        {
            Equip(item);
        }
    }

    private void Equip([CanBeNull] GameObject item)
    {
        if (item == null) return;

        GunSystem currentWeaponGunSystem;
        var itemData = item.GetComponent<PickUpController>();

        if (currentWeapon != null)
        {
            if (item.name == currentWeapon.name) return;

            currentWeaponGunSystem = currentWeapon.GetComponent<GunSystem>();
            currentWeaponGunSystem.isReloading = false;
            currentWeapon.SetActive(false);
            itemSlots[(int)currentWeaponGunSystem.mWeaponStyle].sprite = currentWeaponGunSystem.passiveIcon;
            currentWeapon.GetComponent<PickUpController>().equipped = false;
        }

        itemData.equipped = true;

        item.SetActive(true);
        currentWeapon = item;

        currentWeaponGunSystem = currentWeapon.GetComponent<GunSystem>();
        currentWeapon.SetActive(true);
        itemSlots[(int)currentWeaponGunSystem.mWeaponStyle].sprite = currentWeaponGunSystem.activeIcon;

        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.Euler(itemData.defaultRotation.x,
            itemData.defaultRotation.y,
            itemData.defaultRotation.z);

        itemData.rb.isKinematic = true;
        itemData.coll.isTrigger = true;

        itemData.gun.enabled = true;
    }

    public void AddItem(GameObject newWeaponObject)
    {
        var newWeapon = newWeaponObject.GetComponent<GunSystem>();
        var slotNum = (int)newWeapon.mWeaponStyle;

        if (weapons[slotNum] != null)
        {
            RemoveItem(newWeaponObject);
        }

        weapons[slotNum] = newWeaponObject;
        itemSlots[slotNum].sprite = newWeapon.passiveIcon;
    }

    public void RemoveItem(GameObject weapon)
    {
        var index = (int)weapon.GetComponent<GunSystem>().mWeaponStyle;

        weapons[index] = null;
        itemSlots[index].sprite = nullItem;
    }

    [CanBeNull]
    public GameObject GetItem(int index)
    {
        return weapons[index];
    }
}