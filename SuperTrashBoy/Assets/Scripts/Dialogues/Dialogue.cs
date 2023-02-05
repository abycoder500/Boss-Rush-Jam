using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "SuperTrashBoy/Dialogue", order = 0)]
public class Dialogue : ScriptableObject 
{
    [SerializeField] NotificationUI.Notification[] dialogues;

    public NotificationUI.Notification[] GetDialogues()
    {
        return dialogues;
    }    
}
