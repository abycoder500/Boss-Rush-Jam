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
    [SerializeField] GameObject gemPrefab;
    [SerializeField] Transform gemPosition;
    [SerializeField] Projectile laserPrefab;
    [SerializeField] float launchForce = 50f;
    [SerializeField] float timeBetweenAttacks = 1.5f;

    private void Start() 
    {
        Instantiate(gemPrefab, gemPosition);    
    }

    public override void Attack(GameObject instigator, Action AttackFinished)
    {
        Debug.Log("Fire!!!!");
        Projectile laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        laser.transform.parent = null;
        laser.Launch(launchForce * Camera.main.transform.forward, 0f, this.gameObject, this, false);
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
        StartCoroutine(ResetAttack(AttackFinished));
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

    private IEnumerator ResetAttack(Action AttackFinished)
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        AttackFinished();
    }
}
