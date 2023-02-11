using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    private GameObject player;
    private Collider mCollider;

    private bool isKnockingBack = false;

    public float windForce = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (mCollider.bounds.Intersects(player.GetComponent<Collider>().bounds))
        {
            Mover move = player.GetComponent<Mover>();
            move.SetWindForce(new Vector2(transform.forward.x, transform.forward.z) * windForce);
            isKnockingBack = true;
        }
        else if (isKnockingBack)
        {
            Mover move = player.GetComponent<Mover>();
            move.ResetWindForce();
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            Mover move = player.GetComponent<Mover>();
            move.ResetWindForce();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
