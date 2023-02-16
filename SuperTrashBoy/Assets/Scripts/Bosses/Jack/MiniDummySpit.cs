using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDummySpit : MonoBehaviour
{

    public GameObject bullet;

    public float spitRate = 0.5f;
    public float bulletDamage = 5f;
    public float bulletSpeed = 1f;
    public float bulletLifetime = 1f;
    public Vector3 bulletOffset = new Vector3(0, -2);

    private float lastBulletTime = 0;

    private GameObject player;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogError("No player gameobject found");
        animator = GetComponentInChildren<Animator>();
        lastBulletTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Time.time > lastBulletTime + spitRate)
        {
            lastBulletTime = Time.time;
            if (IsPlayerLOS())
            {
                LookAtPlayer();
                SpitBullet();
            }
        }
        else if (Time.time > lastBulletTime + (spitRate/2))
        {
            animator.ResetTrigger("isSpitting");
        }
    }

    private bool IsPlayerLOS()
    {
        //Draw a ray from the boss to the player, and return false if it hits anything else
        Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit);
        if (hit.collider.gameObject != player.GetComponent<Collider>().gameObject)
            return false;
        else
            return true;
    }

    private void SpitBullet()
    {
        GameObject bulletInst = Instantiate(bullet, transform.position + bulletOffset, transform.rotation);
        bulletInst.GetComponent<Bullet>().SetUpBullet(bulletLifetime, bulletSpeed, bulletDamage, gameObject);
        animator.SetTrigger("isSpitting");
    }

    private void LookAtPlayer()
    {
        transform.LookAt(player.transform);
        //Make sure Jack isn't tilting up or down
        //transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y);
    }
}
