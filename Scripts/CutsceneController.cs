using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public GameObject[] animationScenes;

    bool cutsceneOver;
    bool returningToMenu;

    private void Start()
    {
        cutsceneOver = false;
        returningToMenu = false;

        for (int i = 0; i < animationScenes.Length; i++)
        {
            animationScenes[i].SetActive(false);
        }

        // Rumble Noises
        FindObjectOfType<AudioController>().SetMusic("Deep Rumble");

        StartCoroutine(PlayAnimations());
    }

    private void Update()
    {
        if (cutsceneOver 
            && !returningToMenu
            && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ReturnToMenu());
        }
    }

    public IEnumerator PlayAnimations()
    {
        animationScenes[0].SetActive(true);

        yield return new WaitForSeconds(13.5f);

        animationScenes[0].SetActive(false);
        animationScenes[1].SetActive(true);

        yield return new WaitForSeconds(4.5f);

        // Moment of silence before entering final scene
        FindObjectOfType<AudioController>().SetMusic("None");

        yield return new WaitForSeconds(2f);

        animationScenes[1].SetActive(false);
        animationScenes[2].SetActive(true);

        // Park music
        FindObjectOfType<AudioController>().SetMusic("Park Ambience");

        yield return new WaitForSeconds(7f);

        cutsceneOver = true;

        yield return null;
    }

    public IEnumerator ReturnToMenu()
    {
        returningToMenu = true;

        animationScenes[2].GetComponent<Animator>().SetTrigger("ToMenu");

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(0);

        yield return null;
    }
}
