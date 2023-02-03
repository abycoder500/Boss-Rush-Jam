using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public HitBox hitbox;
    private GameObject instigator;
    private float damage;
    private float speed;
    private float lifetime;
    private float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetUpBullet(float inLifetime, float inSpeed, float inDamage, GameObject inInstigator)
    {
        instigator = inInstigator;
        lifetime = inLifetime;
        speed = inSpeed;
        damage = inDamage;
        hitbox.SetupHitBox(instigator, damage);
        spawnTime = Time.time;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed;
        if (Time.time > spawnTime + lifetime)
            Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
