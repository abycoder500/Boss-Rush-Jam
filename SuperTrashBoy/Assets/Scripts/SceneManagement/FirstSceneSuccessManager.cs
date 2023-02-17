using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneSuccessManager : MonoBehaviour
{
    private MySceneManager mySceneManager;

    [SerializeField] private int eventsToConcludeScene = 2;
    [SerializeField] private float timeToLoadNextScene = 10f;
    [SerializeField] private NotificationUI notificationUI;
    
    private Pickable gemPickable;
    private Pickable rangedWeaponPickable;

    private int eventsHappened = 0;

    private void Awake()
    {
        mySceneManager = FindObjectOfType<MySceneManager>();    
    }

    public void SetGemAndWeapon(Pickable gem, Pickable weapon)
    {
        gemPickable = gem;
        rangedWeaponPickable = weapon;

        gem.onCollected += AddEvent;
        weapon.onCollected += AddEvent;
    }

    private void AddEvent()
    {
        Debug.Log("event");
        eventsHappened ++;
        if(eventsHappened == eventsToConcludeScene) 
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitUntil(() => !notificationUI.IsShowing());
        yield return new WaitForSeconds(timeToLoadNextScene);
        mySceneManager.LoadJackScene();
    }

}
