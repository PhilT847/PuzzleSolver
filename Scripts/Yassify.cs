using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Yassify : ScreenEvent
{
    public float currentValue;

    public bool pressedButton;

    public RectTransform pill;

    public float gameSpeed; // speed of the pill. Randomized to reduce monotony

    bool movingRight;

    bool playedGulpSound;

    private void Start()
    {
        // 0 (right), 1 (middle), 2 (left)
        int randomizeValue = Random.Range(0, 3);

        if(randomizeValue == 0)
        {
            currentValue = -99f;
            movingRight = true;
            pressedButton = false;
        }
        else if(randomizeValue == 1)
        {
            currentValue = 0f;
            movingRight = true;
            pressedButton = false;
        }
        else
        {
            currentValue = 99f;
            movingRight = false;
            pressedButton = false;
        }

        // Randomize speed
        gameSpeed = Random.Range(450f, 550f);

        // Set position based on value
        pill.anchoredPosition = new Vector3(currentValue * 1.4f, pill.anchoredPosition.y, 0f);

        FindObjectOfType<AudioController>().Play("ManCry");
    }

    private void Update()
    {
        // Select so long as timer isn't up
        if (!pressedButton)
        { 
            MovePill();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                pressedButton = true;
                FindObjectOfType<Event>().eventDuration = 0.05f; // Force out the event's end
            }
        }
    }

    void MovePill()
    {
        if(movingRight)
        {
            if(currentValue < 100f) // move to right
            {
                currentValue += gameSpeed * Time.deltaTime;
            }
            else // flip motion
            {
                movingRight = !movingRight;
            }
        }
        else
        {
            if (currentValue > -100f) // move to right
            {
                currentValue -= gameSpeed * Time.deltaTime;
            }
            else // flip motion
            {
                movingRight = !movingRight;
            }
        }

        // Set position based on value
        pill.anchoredPosition = new Vector3(currentValue * 1.4f, pill.anchoredPosition.y, 0f);
    }

    public IEnumerator PressButton()
    {
        // Prevent any more actions
        pressedButton = true;

        FindObjectOfType<AudioController>().Play("Impact");

        // Set timer as to not spawn next event during end
        FindObjectOfType<ThingTimer>().timeBeforeThing = 99f;

        // Win if value is low enough (0-55 out of 100)
        bool playerWon = (Mathf.Abs(currentValue) < 55f);

        // Pill falls
        while (pill.anchoredPosition.y > -100f)
        {
            pill.anchoredPosition = new Vector3(pill.anchoredPosition.x, pill.anchoredPosition.y - 500f * Time.deltaTime, 0f);

            // If won, make pill clear when touching Jup
            if(pill.anchoredPosition.y < -50f 
                && playerWon
                && !playedGulpSound)
            {
                playedGulpSound = true;

                pill.GetComponentInChildren<Image>().color = Color.clear;

                FindObjectOfType<AudioController>().Play("PillGulp");
            }

            yield return new WaitForEndOfFrame();
        }

        pill.GetComponentInChildren<Image>().color = Color.clear;

        // Animate
        if (playerWon)
        {
            GetComponentInChildren<Animator>().SetTrigger("Win");

            FindObjectOfType<AudioController>().Play("SexyOh");
            FindObjectOfType<AudioController>().Play("WolfWhistle");

            yield return new WaitForSeconds(1.24f);

            FindObjectOfType<GameController>().WinEvent();
        }
        else
        {
            GetComponentInChildren<Animator>().SetTrigger("Lose");

            FindObjectOfType<AudioController>().Play("PillLose");

            yield return new WaitForSeconds(0.98f);

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
        StartCoroutine(PressButton());
    }
}
