using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTrashBoy.BehaviorTrees;
using System;

public class BTUser : MonoBehaviour
{
    public enum BehaviorState {IDLE, WORKING};

    protected GameObject player;
    protected Vector3 playerLocation;
    protected bool paused;

    protected BehaviorTree bt;
    public BehaviorState state = BehaviorState.IDLE;
    public Node.Status status = Node.Status.RUNNING;

    protected virtual void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");    
    }

    protected virtual void Start()
    {
        bt = new BehaviorTree("My Tree");
    }

    // Update is called once per frame
    void Update()
    {
        if(paused) return;
        if(status != Node.Status.SUCCESS) status = bt.Process();
    }

    protected Node.Status FindPlayerLocation()
    {
        if(player == null) return Node.Status.FAILURE;

        playerLocation = player.transform.position;
        return Node.Status.SUCCESS;
    }

    protected bool IsPlayerWithinRange(float range)
    {
        return Vector3.Distance(playerLocation,transform.position) <= range;
    }

    public void Pause(bool pause)
    {
        paused = pause;
    }

    public void Pause(float time)
    {
        StartCoroutine(PauseRoutine(time));
    }

    private IEnumerator PauseRoutine(float time)
    {
        paused = true;
        yield return new WaitForSeconds(time);
        paused = false;
    }
}
