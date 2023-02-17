using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedBoxScript : MonoBehaviour
{
    [SerializeField] private Pickable gemPrefab;
    private bool isFake = true;
    private bool isFinalBox = false;

    private FightManager manager;
    private DialogueTrigger endDialogue;
    private NotificationUI notificationUI;
    private AudioManager audioManager;

    private void Awake() 
    {
        endDialogue = GameObject.FindGameObjectWithTag("EndDialogue").GetComponent<DialogueTrigger>();    
        notificationUI = FindObjectOfType<NotificationUI>();
        audioManager = FindObjectOfType<AudioManager>();
    }

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

    public void SetFinalBox()
    {
        isFinalBox = true;
        isFake = false;
    }

    private void AwakeBoxes()
    {
        if (isFinalBox)
        {
            HandleBattleEnd();
            return;
        }

        Activate();

        if (isFake == true)
        {
            manager.AwakeBoxes();
        }
        else
        {
            manager.RemoveBoxes();
        }
    }

    public void Activate()
    {
        if (isFake)
        {
            manager.SpawnMiniJack(transform.parent.transform);
        }
        else
        {
            manager.SpawnJack(transform.parent.transform);
        }
        Destroy(transform.parent.gameObject);
    }

    private void HandleBattleEnd()
    {
        if(gemPrefab != null) 
        {
            Pickable gem = Instantiate(gemPrefab, transform.position, Quaternion.identity);
            gem.transform.parent = null;
        }
        Debug.Log("A winner is you!");
        endDialogue.StartDialogue();
        StartCoroutine(SceneEnd());
    }

    private IEnumerator SceneEnd()
    {
        audioManager.StopMusic(null);
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !notificationUI.IsShowing());
        MySceneManager mySceneManager = FindObjectOfType<MySceneManager>();
        mySceneManager.NextScene();
    }
}
