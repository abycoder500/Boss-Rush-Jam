using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    [System.Serializable]
    public struct Notification
    {
        public string text;
        public float timeOnScreen;
        public bool sound;
    }

    [SerializeField] Image panel;
    [SerializeField] TextMeshProUGUI notificationText;
    [SerializeField] float disappearanceTime = 0.5f;
    [SerializeField] float timeToShow = 1f;
    [SerializeField] float timeBetweenNotifications = 1f;
    [SerializeField] AudioClip notificationSound = null;

    private Coroutine activeNotification = null;
    private AudioSource audioSource;
    private bool isShowing = false;

    private List<Notification> notificationsOnQueue = new List<Notification>();

    Notification test1;
    Notification test2;
    Notification test3;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();    
    }


    private void Start()
    {
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
        notificationText.alpha = 0;
    }

    public void ShowNotification(Notification notification)
    {
        if(activeNotification == null)
        {
            isShowing = true;
            activeNotification = StartCoroutine(ShowText(notification));
            if(notification.sound && notificationSound != null) audioSource.PlayOneShot(notificationSound);
        }
        else
        {
            notificationsOnQueue.Add(notification);
        }
    }

    private IEnumerator ShowText(Notification notification)
    {
        notificationText.text = notification.text;
        StartCoroutine(ChangePanelAlpha(1f, timeToShow));
        yield return StartCoroutine(ChangeTextAlpha(1f, timeToShow));
        yield return new WaitForSeconds(notification.timeOnScreen);
        StartCoroutine(ChangePanelAlpha(0f, disappearanceTime));
        yield return StartCoroutine(ChangeTextAlpha(0f, disappearanceTime));
        activeNotification = null;
        if(notificationsOnQueue.Contains(notification)) notificationsOnQueue.Remove(notification);
        if(notificationsOnQueue.Count >= 1) 
        {
            yield return new WaitForSeconds(timeBetweenNotifications);
            ShowNotification(notificationsOnQueue[0]);
        }
        else
        {
            isShowing = false;
        }
    }

    private IEnumerator ChangePanelAlpha(float target, float time)
    {
        while (!Mathf.Approximately(panel.color.a, target))
        {
            float a = Mathf.MoveTowards(panel.color.a, target, Time.unscaledDeltaTime / time);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, a);
            yield return null;
        }
    }

    private IEnumerator ChangeTextAlpha(float target, float time)
    {
        while (!Mathf.Approximately(notificationText.alpha, target))
        {
            float a = Mathf.MoveTowards(notificationText.alpha, target, Time.unscaledDeltaTime / time);
            notificationText.alpha = a;
            yield return null;
        }
    }

    public bool IsShowing()
    {
        return isShowing;
    }

    
}
