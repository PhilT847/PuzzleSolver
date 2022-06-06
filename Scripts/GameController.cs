using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public Event[] allEvents;
    public List<Event> availableEvents; // Events left in the cycle
    public bool readyForNextEvent; // prevents multiple event spawns when timer runs out

    public TextMeshProUGUI promptText;
    public Animator promptAnim;

    public GameObject gameOverObject;
    public GameObject winGameObject;

    // Strings in text based on winning/losing
    public string[] wonStrings;
    public string[] lostStrings;

    public Slider healthSlider;
    public Animator healthAnim;

    // Blackout appears when winning/losing
    public Animator blackoutAnim;

    public Background gameBackground;

    public GameObject spacePrompt; // Shown when a ScreenEvent opens
    public GameObject arrowsPrompt;
    public Animator eventAnim;

    // Objects active once the game becomes corrupted
    public bool gameCorrupted;
    public GameObject[] activatedOnCorruption;
    public GameObject[] removedOnCorruption;

    public Image damageBar; // red overlay based on damage taken

    private void Awake()
    {
        StartCoroutine(BeginGame());
    }

    // Set up the puzzle game and move the character to position
    public IEnumerator BeginGame()
    {
        // Play puzzle music
        FindObjectOfType<AudioController>().SetMusic("Puzzle Theme");

        FindObjectOfType<MainCharacter>().transform.position = new Vector3(-4.5f, -0.5f, 0f);

        for (int i = 0; i < activatedOnCorruption.Length; i++)
        {
            activatedOnCorruption[i].gameObject.SetActive(false);
        }

        // Begin event list
        for (int j = 0; j < allEvents.Length; j++)
        {
            availableEvents.Add(allEvents[j]);
        }

        gameCorrupted = false;

        // Can't move until map is set up
        FindObjectOfType<MainCharacter>().canMove = false;

        yield return new WaitForSeconds(1.5f);

        FindObjectOfType<MainCharacter>().canMove = true;

        yield return null;
    }

    public void CorruptGame()
    {
        gameCorrupted = true;
        readyForNextEvent = true; // Allow events to begin

        for (int i = 0; i < activatedOnCorruption.Length; i++)
        {
            activatedOnCorruption[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < removedOnCorruption.Length; i++)
        {
            Destroy(removedOnCorruption[i].gameObject);
        }

        FindObjectOfType<AudioController>().SetMusic("Dom Song", "Background Rain");
    }

    public IEnumerator BeginRandomEvent()
    {
        // Prevent multiple event spawns
        readyForNextEvent = false;

        // Set time to infinite while waiting for event
        FindObjectOfType<ThingTimer>().timeBeforeThing = 99f;

        // Select random event
        int choiceIndex = Random.Range(0, availableEvents.Count);

        // Set prompt text and animate before beginning
        promptText.SetText(availableEvents[choiceIndex].eventPrompt);
        promptAnim.SetTrigger("NewGame");

        // Remove ballGoal until event is complete
        GetComponent<BallController>().ballGoal.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.6f);

        Event chosenEvent = Instantiate(availableEvents[choiceIndex], transform.position, Quaternion.identity);

        // Remove event after spawning
        availableEvents.RemoveAt(choiceIndex);

        // If out of events, reset list
        if (availableEvents.Count == 0)
        {
            for (int j = 0; j < allEvents.Length; j++)
            {
                availableEvents.Add(allEvents[j]);
            }
        }

        eventAnim.SetTrigger("Start");

        // Set timer maximum
        FindObjectOfType<ThingTimer>().clockSlider.maxValue = chosenEvent.eventDuration;

        yield return null;
    }

    /*
    public void BeginRandomEvent()
    {
        int choiceIndex = Random.Range(0, availableEvents.Count);

        Event chosenEvent = Instantiate(availableEvents[choiceIndex], transform.position, Quaternion.identity);

        availableEvents.RemoveAt(choiceIndex);

        // If out of events, reset list
        if(availableEvents.Count == 0)
        {
            for (int j = 0; j < allEvents.Length; j++)
            {
                availableEvents.Add(allEvents[j]);
            }
        }

        // Remove ballGoal until event is complete
        GetComponent<BallController>().ballGoal.gameObject.SetActive(false);

        eventAnim.SetTrigger("Start");
    }
    */

    public void WinEvent()
    {
        eventAnim.SetTrigger("Win");

        FindObjectOfType<AudioController>().Play("EventWin");

        promptText.SetText(wonStrings[Random.Range(0, wonStrings.Length)]);

        EndEvent();
    }

    public void LoseEvent()
    {
        eventAnim.SetTrigger("Lose");
        
        //FindObjectOfType<AudioController>().Play("GirlDamage");

        promptText.SetText(lostStrings[Random.Range(0, lostStrings.Length)]);

        // Take damage when losing
        FindObjectOfType<MainCharacter>().TakeDamage(1);
        healthAnim.SetTrigger("Hurt");

        EndEvent();
    }

    // Actions that occur whenever an event ends, win or lose
    void EndEvent()
    {
        FindObjectOfType<MainCharacter>().canMove = true;

        FindObjectOfType<ThingTimer>().timeBeforeThing = 4f;

        gameBackground.SelectRandomBackground();

        // Return ballGoal
        GetComponent<BallController>().ballGoal.gameObject.SetActive(true);

        GetComponent<DamageOrbSpawner>().BeginWave();

        readyForNextEvent = true;
    }

    public void CharacterDeath()
    {
        StartCoroutine(GameOver());
    }

    public IEnumerator GameOver()
    {
        FindObjectOfType<ThingTimer>().timeBeforeThing = 9999f;

        blackoutAnim.SetTrigger("Blackout");

        // Destroy ball goal so ball cannot score when losing
        GetComponent<BallController>().ballGoal.gameObject.SetActive(false);

        // Destroy any active events
        if (FindObjectOfType<Event>())
        {
            Destroy(FindObjectOfType<Event>().gameObject);
        }

        yield return new WaitForSeconds(1f);

        gameOverObject.SetActive(true);

        FindObjectOfType<AudioController>().SetMusic("Lose Theme");

        Time.timeScale = 0f;

        yield return null;
    }

    public IEnumerator WinGame()
    {
        FindObjectOfType<ThingTimer>().timeBeforeThing = 9999f;

        blackoutAnim.SetTrigger("Blackout");

        // Destroy any active events
        if (FindObjectOfType<Event>())
        {
            Destroy(FindObjectOfType<Event>().gameObject);
        }

        // Heal fully to ensure any possible queued damage events don't kill
        FindObjectOfType<MainCharacter>().currentHP = 4;
        FindObjectOfType<MainCharacter>().canMove = false;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(2);

        /*
        winGameObject.SetActive(true);

        FindObjectOfType<AudioController>().SetMusic("Win Theme");
        */

        yield return null;
    }

    public void RestartGame()
    {
        // Ensure timeScale is correct
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
