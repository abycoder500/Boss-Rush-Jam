using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstDialogue : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;
    private GameObject player;

    [SerializeField] float timeToGivePlayerControl = 5f;

    private void Awake() 
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();    
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private IEnumerator Start() 
    {
        yield return new WaitForSeconds(0.1f);
        player.GetComponent<PlayerController>().enabled = false;
        dialogueTrigger.StartDialogue();    
        yield return new WaitForSeconds(timeToGivePlayerControl);
        player.GetComponent<PlayerController>().enabled = true;
    }
}
