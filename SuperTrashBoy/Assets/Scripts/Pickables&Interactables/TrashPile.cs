using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashPile : MonoBehaviour
{
    [SerializeField] private HealthPickable healthPickablePrefab;
    [SerializeField] private int numberOfHealthSpawn;
    [SerializeField] private int numberOfWeaponPartsSpawn;
    [SerializeField] private float spawnForce;
    [SerializeField] private float verticalSpawnAngle = 45f;

    public void SpawnPickables()
    {
        for (int i = 0; i < numberOfHealthSpawn; i++)
        {
            HealthPickable instance = Instantiate(healthPickablePrefab, transform.position, Quaternion.identity);

            float xdir = Random.Range(0f, 1f);
            float zdir = Random.Range(0f, 1f);
            float ydir = Mathf.Sin(verticalSpawnAngle * Mathf.PI / 180f);
            Vector3 spawnDir = new Vector3(xdir,ydir,zdir);
            spawnDir.Normalize();
            //Debug.Log(spawnDir);
            instance.Spawn(spawnForce * spawnDir);
        }     
    }
}
