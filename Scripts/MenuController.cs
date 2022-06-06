using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    bool startingGame;

    private void Start()
    {
        // Play puzzle music
        FindObjectOfType<AudioController>().SetMusic("Menu Theme");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !startingGame)
        {
            StartCoroutine(StartGame());
        }
    }

    public IEnumerator StartGame()
    {
        FindObjectOfType<AudioController>().Play("BallGoal");

        startingGame = true;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(1);

        yield return null;
    }
}
