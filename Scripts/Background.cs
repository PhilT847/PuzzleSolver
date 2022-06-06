using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject[] possibleBackgrounds;

    public int currentBackground;

    private void Start()
    {
        // Set random to start
        for(int i = 0; i < possibleBackgrounds.Length; i++)
        {
            possibleBackgrounds[i].SetActive(false);
        }

        SelectRandomBackground();
    }

    public void SelectRandomBackground()
    {
        int randomizedRoll = Random.Range(0, possibleBackgrounds.Length);

        // Ensure a new background comes in, if there are multiple
        if (possibleBackgrounds.Length > 1)
        {
            while (randomizedRoll == currentBackground)
            {
                randomizedRoll = Random.Range(0, possibleBackgrounds.Length);
            }
        }

        // Set current background inactive, if one is active
        if(possibleBackgrounds[currentBackground].activeSelf)
        {
            possibleBackgrounds[currentBackground].SetActive(false);
        }

        currentBackground = randomizedRoll;

        for(int i = 0; i < possibleBackgrounds.Length; i++)
        {
            // Find selected background and return
            if(i == currentBackground)
            {
                possibleBackgrounds[i].SetActive(true);
                return;
            }
        }
    }
}
