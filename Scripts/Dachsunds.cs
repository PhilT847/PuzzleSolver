using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dachsunds : ScreenEvent
{
    public bool selectedDachsund;

    public int playerChosenAnswer;
    public int correctIndex;

    public RectTransform pointer;

    public TextMeshProUGUI chosenColorText;

    private void Start()
    {
        CreateBoard();
    }

    private void Update()
    {
        // Move and select so long as timer isn't up
        if (!selectedDachsund)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (playerChosenAnswer == 2)
                {
                    MoveToAnswer(0);
                }
                else
                {
                    MoveToAnswer(playerChosenAnswer + 1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (playerChosenAnswer == 0)
                {
                    MoveToAnswer(2);
                }
                else
                {
                    MoveToAnswer(playerChosenAnswer - 1);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectedDachsund = true;
                FindObjectOfType<Event>().eventDuration = 0.05f; // Force out the AttemptSolve
            }
        }
    }

    void CreateBoard()
    {
        selectedDachsund = false;

        // Pick a color
        int correctNdx = Random.Range(0, 3);

        // Set correct index
        correctIndex = correctNdx;

        switch(correctIndex)
        {
            case 0: // red
                chosenColorText.SetText("RED");
                chosenColorText.color = Color.blue;
                break;
            case 1: // blue
                chosenColorText.SetText("BLUE");
                chosenColorText.color = Color.red;
                break;
            case 2: // yellow
                chosenColorText.SetText("YELLOW");
                chosenColorText.color = Color.green;
                break;
        }

        FindObjectOfType<AudioController>().Play("DogsAngry");

        MoveToAnswer(0);
    }

    void MoveToAnswer(int index)
    {
        playerChosenAnswer = index;

        FindObjectOfType<AudioController>().Play("MoveCursor");

        pointer.localPosition = new Vector3(-135f + (135f * index), 40f, 0f);
    }

    public IEnumerator AttemptSolve()
    {
        // Prevent any more motion
        selectedDachsund = true;

        // Set timer as to not spawn next event during end
        FindObjectOfType<ThingTimer>().timeBeforeThing = 99f;

        bool playerWon = (playerChosenAnswer == correctIndex);

        FindObjectOfType<AudioController>().Play("DogSingle");

        if (playerChosenAnswer == 0)
        {
            GetComponent<Animator>().SetTrigger("Red");
        }
        else if(playerChosenAnswer == 1)
        {
            GetComponent<Animator>().SetTrigger("Blue");
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Yellow");
        }

        yield return new WaitForSeconds(0.74f);

        // Win or lose in GameController
        if (playerWon)
        {
            FindObjectOfType<GameController>().WinEvent();
        }
        else
        {
            FindObjectOfType<GameController>().LoseEvent();
        }

        // Destroy parent event object, thus destroying this object as well
        Destroy(FindObjectOfType<Event>().gameObject);

        yield return null;
    }

    public override void EndScreenEvent()
    {
        StartCoroutine(AttemptSolve());
    }
}
