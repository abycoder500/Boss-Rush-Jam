using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee Weapon", menuName = "SuperTrashBoy/Melee Weapon", order = 0)]
public class MeleeWeapon : Weapon
{
    [SerializeField] private AnimationClip attackAnimation;
    [SerializeField] private float damage;

    public override void Attack(GameObject instigator)
    {
        HitBox hitBox = GetWeaponPrefab().GetComponent<HitBox>();

        hitBox.ActivateHitBox(true, instigator, damage);
    }
}
