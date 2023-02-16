using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgradePickable : Pickable
{
    [SerializeField] Weapon weapon;
    [SerializeField] NotificationUI.Notification notificationOnSpawn;
    [SerializeField] NotificationUI.Notification notificationOnCollect;

    private Rigidbody rb;

    private NotificationUI notificationUI = null;

    private void Awake() 
    {
        notificationUI = FindObjectOfType<NotificationUI>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Start() 
    {
        base.Start();
        if (rb == null) return;
        rb.useGravity = true;
        rb.isKinematic = false;    
    }

    public void Spawn(Vector3 force)
    {
        if(notificationUI == null) notificationUI = FindObjectOfType<NotificationUI>();
        notificationUI.ShowNotification(notificationOnSpawn);
        if(rb == null) return;
        rb.AddForce(force, ForceMode.Impulse);
    }

    protected override void Collect()
    {
        if (player == null) return;
        notificationUI.ShowNotification(notificationOnCollect);
        player.GetComponent<Fighter>().EquipWeapon(weapon);
        base.Collect();
    }
}
