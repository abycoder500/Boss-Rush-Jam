using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    private GameObject player;
    private Collider mCollider;

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
            //Move player
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
