using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] private Weapon startingWeapon;
    [SerializeField] private Transform weaponLocation;
    [SerializeField] private Animator animator;
  
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
        animator.SetTrigger("Attack");
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (weapon != null) 
        {
            equippedWeapon  = Instantiate(weapon, weaponLocation);
            animator.runtimeAnimatorController = equippedWeapon.GetAnimatorOverride();
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
