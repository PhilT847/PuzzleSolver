using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MathProblem : ScreenEvent
{
    public int integer1;
    public int integer2;

    public TextMeshProUGUI blackboardText;

    public TextMeshProUGUI[] allAnswers;

    public int correctIndex;
    public int playerChosenAnswer;

    public RectTransform pointer;

    private bool answerAttempted;

    private void Start()
    {
        CreateBoard();
    }

    private void Update()
    {
        // Move and select so long as timer isn't up
        if (!answerAttempted)
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
                answerAttempted = true;
                FindObjectOfType<AudioController>().Play("Impact");
                FindObjectOfType<Event>().eventDuration = 0.05f; // Force out the AttemptSolve
            }
        }
    }

    void CreateBoard()
    {
        answerAttempted = false;

        integer1 = Random.Range(1, 10);
        integer2 = Random.Range(1, 10);

        blackboardText.SetText("{0} x {1} = ?", integer1, integer2);

        int correctNdx = Random.Range(0, 3);

        for(int i = 0; i < allAnswers.Length; i++)
        {
            if(i == correctNdx)
            {
                allAnswers[i].SetText("{0}", integer1 * integer2);
            }
            else
            {
                int otherAnswer = Random.Range(0, 100);

                // If the same as the answer, keep rerolling
                while (otherAnswer == integer1 * integer2)
                {
                    otherAnswer = Random.Range(0, 100);
                }

                allAnswers[i].SetText("{0}", otherAnswer);
            }
        }

        // Set correct index
        correctIndex = correctNdx;

        FindObjectOfType<AudioController>().Play("QuestionHmm");

        MoveToAnswer(0);
    }

    void MoveToAnswer(int index)
    {
        playerChosenAnswer = index;

        FindObjectOfType<AudioController>().Play("MoveCursor");

        pointer.localPosition = new Vector3(-85f + (85f * index), 30f, 0f);
    }

    public IEnumerator AttemptSolve()
    {
        // Prevent any more motion
        answerAttempted = true;

        // Set timer as to not spawn next event during end
        FindObjectOfType<ThingTimer>().timeBeforeThing = 99f;

        bool playerWon = (playerChosenAnswer == correctIndex);

        // Animate
        if(playerWon)
        {
            GetComponentInChildren<Animator>().SetTrigger("Win");
            FindObjectOfType<AudioController>().Play("RightAnswer");
        }
        else
        {
            GetComponentInChildren<Animator>().SetTrigger("Lose");
            FindObjectOfType<AudioController>().Play("WrongAnswer");
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
