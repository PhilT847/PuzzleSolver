using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrbSpawner : MonoBehaviour
{
    // Rows where skulls can spawn
    public Transform[] spawnPlaces;

    public GameObject damageOrbPrefab;

    // Level controls speed, waves, etc... determined by BallController's ballCount
    // 1 (1,2,3), 2 (4,5), 3 (6,7)
    public int level;

    public void DetermineLevel(int ballCount)
    {
        if (ballCount < 4)
        {
            level = 1;
        }
        else if (ballCount < 6)
        {
            level = 2;
        }
        else
        {
            level = 3;
        }
    }

    void SpawnSkulls()
    {
        List<int> takenSpots = new List<int>();

        int spawnAmount = 0;

        if(level == 1)
        {
            spawnAmount = 2;
        }
        else
        {
            spawnAmount = 3;
        }

        for (int i = 0; i < spawnAmount; i++)
        {
            int rando = Random.Range(0, spawnPlaces.Length);

            // Ensure a unique spot each time
            while (takenSpots.Contains(rando))
            {
                rando = Random.Range(0, spawnPlaces.Length);
            }

            // Add to list to prevent multiples
            takenSpots.Add(rando);

            DamageOrb newSkull = Instantiate(damageOrbPrefab, spawnPlaces[rando].position, Quaternion.identity).GetComponent<DamageOrb>();

            // Create movespeed based on level, with slight variation
            newSkull.moveSpeed = 4f + (level * 2) + Random.Range(-0.5f,0f);

            // Flip position/speed if on the right side
            if (spawnPlaces[rando].position.x > 0f)
            {
                newSkull.flippedSprite.transform.localScale = new Vector3(newSkull.flippedSprite.transform.localScale.x * -1f, 
                                                                            newSkull.flippedSprite.transform.localScale.y, 1f);

                newSkull.direction = Vector2.left;
            }
            else
            {
                newSkull.direction = Vector2.right;
            }
        }
    }

    public void BeginWave()
    {
        StartCoroutine(WaveOfSkulls());
    }

    public IEnumerator WaveOfSkulls()
    {
        yield return new WaitForSeconds(0.75f);

        SpawnSkulls();

        if(level == 2)
        {
            yield return new WaitForSeconds(1f);

            SpawnSkulls();
        }

        if(level == 3)
        {
            yield return new WaitForSeconds(0.8f);

            SpawnSkulls();

            yield return new WaitForSeconds(0.8f);

            SpawnSkulls();
        }

        yield return null;
    }
}
