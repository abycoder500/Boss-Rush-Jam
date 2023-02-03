using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelExplode : MonoBehaviour
{
    [SerializeField] float explodeHeight = -4f;
    [SerializeField] float explodeTime = 1.5f;

    public HitBox hitBox;
    public GameObject explodeEffect;

    private GameObject instigator;
    private float damage;
    private float explodeTimePoint = 0f;

    private bool hasExploaded = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetupAttack(GameObject instigator, float damage)
    {
        this.instigator = instigator;
        this.damage = damage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (transform.position.y < explodeHeight)
        {
            Explode();
        }
        
        if (Time.time > explodeTimePoint + explodeTime && hasExploaded)
        {
            //Explosion has finished, destroy the barrel
            Destroy(this.gameObject);
        }

    }

    public void Explode()
    {
        if (hasExploaded)
        {
            return;
        }
        Rigidbody rd = GetComponent<Rigidbody>();
        if (rd != null)
        {
            rd.useGravity = false;
            rd.velocity = Vector3.zero;
        }
        hasExploaded = true;
        hitBox.SetupHitBox(instigator, damage);
        hitBox.gameObject.SetActive(true);
        explodeEffect.SetActive(true);
        explodeTimePoint = Time.time;
    }
}
