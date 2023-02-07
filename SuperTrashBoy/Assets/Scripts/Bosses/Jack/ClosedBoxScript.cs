using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedBoxScript : MonoBehaviour
{
    private bool isFake = true;
    private FightManager manager;
    // Start is called before the first frame update
    void Start()
    {
        HitReceivedCounter hitReceiver = GetComponent<HitReceivedCounter>();
        hitReceiver.onHitEvent.AddListener(AwakeBoxes);
        manager = FindObjectOfType<FightManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpBox(bool isNotReal)
    {
        isFake = isNotReal;
    }

    private void AwakeBoxes()
    {
        if (isFake == true)
            manager.AwakeBoxes();
        else
            manager.RemoveBoxes();
            Activate();
    }

    public void Activate()
    {
        if (isFake)
        {
            manager.SpawnMiniJack(transform);
        }
        else
        {
            manager.SpawnJack(transform);
        }
        Destroy(gameObject);
    }
}
