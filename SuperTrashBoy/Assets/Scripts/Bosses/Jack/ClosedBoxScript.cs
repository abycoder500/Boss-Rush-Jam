using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedBoxScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HitReceivedCounter hitReceiver = GetComponent<HitReceivedCounter>();
        hitReceiver.onHitEvent.AddListener(Activate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Activate()
    {

    }
}
