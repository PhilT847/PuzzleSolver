using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pancake : ScreenEvent
{
    public Image blobSprite;

    // Press up to 6 buttons in a sequence. Messing up brings you back to the start
    public Image[] buttonSequence;

    // Sprite that pushes down on the blob
    public Image fistSprite;

    // Inputs needed (in order)
    public List<KeyCode> neededInputs;

    // 0 (space), 1 (left), 2 (right)
    public Sprite[] allButtonSprites;

    // 0 (normal), 1 (pancakes, win image)
    public Sprite[] allBlobSprites;

    public Sprite[] allFistSprites;

    // Color of non-current buttons
    public Color darkGray = new Color32(80, 80, 80, 255);

    // 0.5s grace period that prevents wrong moves as the game opens
    float timeGameHasRun;

    int currentInput; // 0-3

    bool gameComplete;

    private void Start()
    {
        CreateBoard();
    }

    private void Update()
    {
        timeGameHasRun += Time.deltaTime;

        // Select so long as timer isn't up
        if (!gameComplete)
        {
            // After 0.5s, you can input wrong keys
            if(timeGameHasRun > 0.5f)
            {
                // Wrong inputs
                if (Input.GetKeyDown(KeyCode.Space)
                    && neededInputs[currentInput] != KeyCode.Space)
                {
                    WrongInput();
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow)
                    && neededInputs[currentInput] != KeyCode.LeftArrow)
                {
                    WrongInput();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow)
                    && neededInputs[currentInput] != KeyCode.RightArrow)
                {
                    WrongInput();
                }
            }

            // Correct input
            if (Input.GetKeyDown(neededInputs[currentInput]))
            {
                CorrectInput();
            }
        }
    }

    // Create 6 keys that must be pushed
    void CreateBoard()
    {
        FindObjectOfType<AudioController>().Play("KnifeScrape");

        // Set blob to base sprite
        blobSprite.sprite = allBlobSprites[0];

        for (int i = 0; i < 4; i++)
        {
            int randomKey = Random.Range(0, 3);

            switch(randomKey)
            {
                case 0:
                    neededInputs.Add(KeyCode.Space);
                    buttonSequence[i].sprite = allButtonSprites[0];
                    break;
                case 1:
                    neededInputs.Add(KeyCode.LeftArrow);
                    buttonSequence[i].sprite = allButtonSprites[1];
                    break;
                case 2:
                    neededInputs.Add(KeyCode.RightArrow);
                    buttonSequence[i].sprite = allButtonSprites[2];
                    break;
            }
        }

        // Set first fist sprite
        switch (neededInputs[currentInput])
        {
            case KeyCode.Space:
                fistSprite.sprite = allFistSprites[0];
                break;
            case KeyCode.LeftArrow:
                fistSprite.sprite = allFistSprites[1];
                break;
            case KeyCode.RightArrow:
                fistSprite.sprite = allFistSprites[2];
                break;
        }

        ColorButtons(currentInput);
    }

    void CorrectInput()
    {
        // Turn previous button grey
        buttonSequence[currentInput].color = darkGray;

        currentInput++;

        FindObjectOfType<Event>().points = currentInput;

        // Play slam animation
        GetComponentInChildren<Animator>().SetTrigger("Slam");

        FindObjectOfType<AudioController>().Play("PancakeSlap");
        FindObjectOfType<AudioController>().Play("PancakeSqueal");

        if (currentInput >= 4)
        {
            gameComplete = true;
            FindObjectOfType<Event>().points = 4; // Set to required points
            FindObjectOfType<Event>().eventDuration = 0.05f; // Force out the event's end
        }
        else // Indicate that you must push the next button
        {
            switch (neededInputs[currentInput])
            {
                case KeyCode.Space:
                    fistSprite.sprite = allFistSprites[0];
                    break;
                case KeyCode.LeftArrow:
                    fistSprite.sprite = allFistSprites[1];
                    break;
                case KeyCode.RightArrow:
                    fistSprite.sprite = allFistSprites[2];
                    break;
            }
        }

        // Color buttons
        ColorButtons(currentInput);
    }

    void ColorButtons(int currentIndex)
    {
        for(int i = 0; i < 4; i++)
        {
            if(i == currentIndex)
            {
                buttonSequence[i].color = Color.white;
            }
            else if (i > currentIndex)
            {
                buttonSequence[i].color = darkGray;
            }
            else // Already pushed; make black
            {
                buttonSequence[i].color = Color.clear;
            }
        }
    }

    void WrongInput()
    {
        gameComplete = true;
        FindObjectOfType<Event>().points = 0;
        FindObjectOfType<Event>().eventDuration = 0.05f; // Force out the event's end
    }

    public IEnumerator FinishEvent()
    {
        // Prevent any more actions
        gameComplete = true;

        // Set timer as to not spawn next event during end
        FindObjectOfType<ThingTimer>().timeBeforeThing = 99f;

        // Win if you hit all 6
        bool playerWon = FindObjectOfType<Event>().points >= 4;

        // Animate
        if (playerWon)
        {
            // Fist disappears; is replaced with fork hand
            fistSprite.color = Color.clear;

            // Set new sprite (pancake)
            blobSprite.sprite = allBlobSprites[1];

            FindObjectOfType<AudioController>().Play("Impact");

            yield return new WaitForSeconds(0.25f);

            FindObjectOfType<AudioController>().Play("ManMmm");

            GetComponentInChildren<Animator>().SetTrigger("Win");

            yield return new WaitForSeconds(0.74f);

            FindObjectOfType<GameController>().WinEvent();
        }
        else
        {
            GetComponentInChildren<Animator>().SetTrigger("Lose");

            FindObjectOfType<AudioController>().Play("WrongAnswer");

            yield return new WaitForSeconds(0.74f);

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
        StartCoroutine(FinishEvent());
    }
}
