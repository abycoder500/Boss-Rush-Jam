using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgradePickable : Pickable
{
    [SerializeField] Weapon weapon;
    [SerializeField] NotificationUI.Notification notificationOnSpawn;
    [SerializeField] NotificationUI.Notification notificationOnCollect;

    private NotificationUI notificationUI = null;

    private void Awake() 
    {
        notificationUI = FindObjectOfType<NotificationUI>();
    }

    public void Spawn()
    {
        if(notificationUI == null) notificationUI = FindObjectOfType<NotificationUI>();
        notificationUI.ShowNotification(notificationOnSpawn);
    }

    protected override void Collect()
    {
        notificationUI.ShowNotification(notificationOnCollect);
        player.GetComponent<Fighter>().EquipWeapon(weapon);
        base.Collect();
    }
}
