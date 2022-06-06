using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedBackground : MonoBehaviour
{
    public Image mainImage;

    public int currentImage;
    public Sprite[] allSprites;

    public float waitTime;
    private float currentWait;

    private void Start()
    {
        currentWait = waitTime;
    }

    private void Update()
    {
        if(currentWait > 0f)
        {
            currentWait -= Time.deltaTime;
        }
        else
        {
            // Cycle through images
            if(currentImage >= allSprites.Length - 1)
            {
                currentImage = 0;
            }
            else
            {
                currentImage++;
            }

            mainImage.sprite = allSprites[currentImage];

            currentWait = waitTime;
        }
    }
}
