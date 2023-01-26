using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] private Weapon startingWeapon;
    [SerializeField] private Transform weaponLocation;
  
    private Weapon equippedWeapon;
    private List<Weapon> weapons;

    private bool isAttacking = false;

    private void Start() 
    {
        EquipWeapon(startingWeapon);
    }

    public void Attack()
    {
        if(isAttacking) return;
        isAttacking = true;
        equippedWeapon.Attack(this.gameObject, AttackFinished);
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (weapon != null) 
        {
            equippedWeapon  = Instantiate(weapon, weaponLocation);
        }
    }

    public bool IsAttackingMelee()
    {
        return isAttacking && equippedWeapon as MeleeWeapon;
    }

    private void AttackFinished()
    {
        isAttacking = false;
    }
        
}
