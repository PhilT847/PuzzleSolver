using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_Spawner : MonoBehaviour
{
    public GameObject debugPrefab;

    public float spawnTime;
    public float timeUntilSpawn;

    void Update()
    {
        if(timeUntilSpawn > 0f)
        {
            timeUntilSpawn -= Time.deltaTime;
        }
        else
        {
            timeUntilSpawn = spawnTime;

            if(debugPrefab != null)
            {
                Spawn();
            }
        }
    }

    void Spawn()
    {
        Instantiate(debugPrefab, new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0f), Quaternion.identity);
    }
}
