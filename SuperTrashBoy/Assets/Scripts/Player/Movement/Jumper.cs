using UnityEngine;

public class Jumper : MonoBehaviour 
{
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpTimeLength;
    [SerializeField] private float jumpVelocity;

    private float jumpTimer;
    private float verticalVelocity;

    private bool isJumping = false;

    public void Jump()
    {
        isJumping = true;
    }    

    private void Update() 
    {
        if(!isJumping) return;

        verticalVelocity = 0f;
        jumpTimer += Time.deltaTime;
        float time = jumpTimer / jumpTimeLength;
        verticalVelocity = jumpCurve.Evaluate(time) * jumpVelocity;
        if (time > 1f)
        {
            isJumping = false;
            jumpTimer = 0f;
        }   
    }

    public float GetVerticalVelocity()
    {
        return verticalVelocity;
    }

    public bool IsJumping()
    {
        return isJumping;
    }
}