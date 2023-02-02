using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAttackController : MonoBehaviour
{

    private Vector3 initialPos;
    private float mSpeed = 0f;
    private float mLife = 1f;
    private float mStartTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        initialPos = transform.position;
        mStartTime = Time.time;
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (Time.time > mStartTime + mLife)
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform.position +=  transform.up * mSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpeedAndLifetime(float speed, float life)
    {
        mSpeed = speed;
        mLife = life;
    }
}
