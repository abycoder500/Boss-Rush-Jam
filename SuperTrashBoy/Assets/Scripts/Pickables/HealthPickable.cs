using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickable : Pickable
{
    [SerializeField] private float healAmount;

    protected override void Collect()
    {
        if (player == null) return;

        Health playerHealth = player.GetComponent<Health>();

        if(playerHealth.TryHeal(healAmount)) Destroy(this.gameObject);
    }
}
