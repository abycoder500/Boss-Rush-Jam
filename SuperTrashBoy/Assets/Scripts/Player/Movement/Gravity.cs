using UnityEngine;

public class Gravity : MonoBehaviour 
{
    [SerializeField] private float gravityValue = -29.43f;

    public float GetGravity()
    {
        return gravityValue;
    }
}