using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private NotificationUI notificationUI;

    [SerializeField] Dialogue dialogue;

    private void Awake() 
    {
        notificationUI = FindObjectOfType<NotificationUI>();    
    }

    public void StartDialogue()
    {
        NotificationUI.Notification[] notifications = dialogue.GetDialogues();

        for (int i = 0; i < notifications.Length; i++)
        {
            notificationUI.ShowNotification(notifications[i]);
        }
    }
}
