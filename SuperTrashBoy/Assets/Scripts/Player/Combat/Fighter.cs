using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] private Weapon startingWeapon;
    [SerializeField] private Transform weaponLocation;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject aim;
  
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

    public void RangeAttack()
    {
        if((equippedWeapon as RangedWeapon) == null) return;
        if(isAttacking) return;
        isAttacking = true;
        RangedWeapon rangedWeapon = equippedWeapon as RangedWeapon;
        rangedWeapon.Fire(this.gameObject, AttackFinished);
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (weapon != null) 
        {
            if(equippedWeapon != null) Destroy(equippedWeapon.gameObject);
            equippedWeapon  = Instantiate(weapon, weaponLocation);
            animator.runtimeAnimatorController = equippedWeapon.GetAnimatorOverride();
            if(equippedWeapon as LaserWeapon == null) aim.SetActive(false);
            else aim.SetActive(true);
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
