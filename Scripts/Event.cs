using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Event : MonoBehaviour
{
    public string eventPrompt; // the verb added to the main canvas
    public Sprite eventFace; // face associated with event

    public GameObject spawnedObject;
    public int amountSpawnedAtOnce_Min;
    public int amountSpawnedAtOnce_Max;

    public float eventDuration;
    public float spawnsDuration;

    public float timeBetweenSpawns; // 0 if only one spawn
    private float timeUntilSpawn;


    /* Points do the following...
     * Hitting a hazard reduces points by 1
     * Hitting a certain item might grant a point
     * Must meet PointsNeeded requirement to "Pass"
     */
    public int points;
    public int pointsNeeded;

    // Used when the object is a game outside the main one. Removes player movement
    public bool removePlayerMovement;

    bool eventEnded;


    private void Start()
    {
        BeginEvent();
    }

    void BeginEvent()
    {
        points = 0;

        timeUntilSpawn = 0.25f;

        //FindObjectOfType<GameController>().promptText.SetText(eventPrompt);

        // Give a slight buffer so the next event can play a certain duration afterwards
        FindObjectOfType<ThingTimer>().timeBeforeThing = eventDuration + 0.1f;

        // If only one object is spawned, create it instantly
        if(spawnsDuration == 0f)
        {
            SpawnObject();
        }

        // ScreenEvents remove motion
        if (removePlayerMovement)
        {
            // Add key prompts based on the event
            if (FindObjectOfType<ScreenEvent>().usesArrows)
            {
                FindObjectOfType<GameController>().arrowsPrompt.SetActive(true);
            }
            if (FindObjectOfType<ScreenEvent>().usesSpace)
            {
                FindObjectOfType<GameController>().spacePrompt.SetActive(true);
            }

            FindObjectOfType<MainCharacter>().canMove = false;
            FindObjectOfType<MainCharacter>().bodyAnim.SetBool("Walking", false);
        }
    }

    private void Update()
    {
        if(eventDuration > 0f)
        {
            eventDuration -= Time.deltaTime;

            if (timeBetweenSpawns > 0f && spawnsDuration > 0f)
            {
                if (timeUntilSpawn > 0f)
                {
                    timeUntilSpawn -= Time.deltaTime;
                }
                else
                {
                    timeUntilSpawn = timeBetweenSpawns;
                    SpawnObject();
                }

                spawnsDuration -= Time.deltaTime;
            }
        }
        else if(!eventEnded)
        {
            eventEnded = true;

            FindObjectOfType<ThingTimer>().clockAnim.ResetTrigger("BeginEvent");
            FindObjectOfType<ThingTimer>().clockAnim.SetTrigger("EventEnd");

            // If the event is a ScreenEvent, check the ScreenEvent
            if (FindObjectOfType<ScreenEvent>())
            {
                FindObjectOfType<GameController>().spacePrompt.SetActive(false);
                FindObjectOfType<GameController>().arrowsPrompt.SetActive(false);

                FindObjectOfType<ScreenEvent>().EndScreenEvent();
                return;
            }
            else // Otherwise, simply check points and destroy all objects
            {
                // Reward players that win. Punish players that don't
                if (points >= pointsNeeded)
                {
                    FindObjectOfType<GameController>().WinEvent();
                }
                else
                {
                    FindObjectOfType<GameController>().LoseEvent();
                }

                Destroy(gameObject);
            }
        }
    }

    void SpawnObject()
    {
        int spawnAmt = Random.Range(amountSpawnedAtOnce_Min, amountSpawnedAtOnce_Max + 1);

        for(int i = 0; i < spawnAmt; i++)
        {
            Instantiate(spawnedObject, Vector3.zero, Quaternion.identity, transform);
        }
    }
}
