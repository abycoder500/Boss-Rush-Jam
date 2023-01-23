using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] private Weapon startingWeapon;
    [SerializeField] private Transform weaponLocation;
  
    private Weapon equippedWeapon;
    private List<Weapon> weapons;

    private void Start() 
    {
        EquipWeapon(startingWeapon);
    }

    public void Attack()
    {
        equippedWeapon.Attack(this.gameObject);
    }

    public void EquipWeapon(Weapon weapon)
    {
        equippedWeapon = weapon;
        GameObject weaponModel = weapon.GetWeaponPrefab();
        if (weaponModel != null) Instantiate(weapon.GetWeaponPrefab(), weaponLocation);
    }
        
}
