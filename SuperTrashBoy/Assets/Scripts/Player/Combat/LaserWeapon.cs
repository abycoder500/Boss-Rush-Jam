using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaserWeapon : Weapon
{
    [SerializeField] LayerMask targetLayers;
    [SerializeField] float weaponRange = 200f;
    [SerializeField] float weaponDamage = 10f;

    public override void Attack(GameObject instigator, Action AttackFinished)
    {
        Debug.Log("Fire!!!!");
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 cameraPosition = Camera.main.transform.position;
        //Vector3 direction = (mousePos - cameraPosition).normalized;
        Vector3 direction = Camera.main.transform.forward;
        Ray ray = new Ray(Camera.main.transform.position, direction);
        Debug.DrawRay(cameraPosition, direction, Color.red, 3f);
        if(Physics.Raycast(ray, out RaycastHit hit, weaponRange, targetLayers))
        {
            if(hit.transform.TryGetComponent<Health>(out Health health))
            {
                health.TryTakeDamage(weaponDamage, instigator.transform);
                Debug.Log("found health");
            }
        }
        AttackFinished();
    }

    private void Update()
     {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 cameraPosition = Camera.main.transform.position;
        //Vector3 direction = (mousePos - cameraPosition).normalized;
        Vector3 direction = Camera.main.transform.forward;
        Ray ray = new Ray(Camera.main.transform.position, direction);
        Debug.DrawRay(cameraPosition, direction, Color.red);    
    }
}
