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
            ForceReceiver forceReceiver = player.GetComponent<ForceReceiver>();
            forceReceiver.AddForce(new Vector3(transform.forward.x, 0f, transform.forward.z) * windForce);
            isKnockingBack = true;
        }
        else if (isKnockingBack)
        {
            ForceReceiver forceReceiver = player.GetComponent<ForceReceiver>();
            forceReceiver.RemoveForce(new Vector3(transform.forward.x, 0f, transform.forward.z) * windForce);
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            ForceReceiver forceReceiver = player.GetComponent<ForceReceiver>();
            forceReceiver.RemoveForce(new Vector3(transform.forward.x, 0f, transform.forward.z) * windForce);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
