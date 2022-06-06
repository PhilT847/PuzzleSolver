using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quiet : ScreenEvent
{
    public bool droppedBomb;

    public int pushesNeeded;
    private int currentPushes;

    public RectTransform tapeTransform;

    private void Start()
    {
        droppedBomb = false;

        currentPushes = 0;

        tapeTransform.localScale = new Vector3(1f, 1f, 1f);

        FindObjectOfType<AudioController>().Play("DogsAngry");
    }

    private void Update()
    {
        // Try to drop bomb if timer isn't up
        if (!droppedBomb)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FindObjectOfType<AudioController>().Play("TapeTap");

                currentPushes++;

                // Shrink tape
                tapeTransform.localScale = new Vector3(1f - (currentPushes * 0.035f), 1f, 1f);

                tapeTransform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-30f,30f));
            }

            if(currentPushes >= pushesNeeded)
            {
                droppedBomb = true;

                FindObjectOfType<AudioController>().Play("TapeRip");

                FindObjectOfType<Event>().points = 1; // Add point
                FindObjectOfType<Event>().eventDuration = 0.05f; // Force out the DropBomb

                // Remove tape
                tapeTransform.GetComponent<Image>().color = Color.clear;
            }
        }
    }

    public IEnumerator DropBomb()
    {
        // Prevent any more actions
        droppedBomb = true;

        // Set timer as to not spawn next event during end
        FindObjectOfType<ThingTimer>().timeBeforeThing = 99f;

        // Win if you dropped the bomb
        bool playerWon = (FindObjectOfType<Event>().points > 0);

        // Animate
        if (playerWon)
        {
            GetComponentInChildren<Animator>().SetTrigger("Win");

            yield return new WaitForSeconds(0.33f);

            FindObjectOfType<AudioController>().Play("BombExplode");
            FindObjectOfType<AudioController>().Play("DogWhine");

            yield return new WaitForSeconds(1.41f);

            FindObjectOfType<GameController>().WinEvent();
        }
        else
        {
            GetComponentInChildren<Animator>().SetTrigger("Lose");

            FindObjectOfType<AudioController>().Play("DogWhine");

            yield return new WaitForSeconds(0.66f);

            FindObjectOfType<GameController>().LoseEvent();
        }

        // Destroy parent event object, thus destroying this object as well
        Destroy(FindObjectOfType<Event>().gameObject);

        // Stop all to confirm this only plays once
        StopAllCoroutines();

        yield return null;
    }


    public override void EndScreenEvent()
    {
        StartCoroutine(DropBomb());
    }
}
