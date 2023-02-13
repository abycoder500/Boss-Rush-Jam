using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpKillPlane : MonoBehaviour
{
    HitBox hitBox;
    // Start is called before the first frame update
    void Start()
    {
        hitBox = GetComponent<HitBox>();
        if (hitBox != null)
        {
            hitBox.SetupHitBox(gameObject, 10000);  //Arbitrarily large damage
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetHitbox()
    {
        //Not really needed, but for safetly
        hitBox.gameObject.SetActive(true);
    }
}
