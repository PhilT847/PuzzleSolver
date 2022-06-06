using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThingTimer : MonoBehaviour
{
    public float timeBeforeThing;

    public Slider clockSlider;

    public Animator clockAnim;

    bool panicking;

    private void Update()
    {
        if(timeBeforeThing > 0f)
        {
            if (FindObjectOfType<Event>())
            {
                if(timeBeforeThing < 1f && !panicking)
                {
                    panicking = true;

                    clockAnim.SetTrigger("NearEnd");
                }
            }
            else
            {
                if(clockSlider.maxValue != 4f)
                {
                    clockSlider.maxValue = 4f;
                }
            }

            timeBeforeThing -= Time.deltaTime;

            clockSlider.value = timeBeforeThing;
        }
        else if(FindObjectOfType<GameController>().readyForNextEvent)
        {
            FindObjectOfType<GameController>().StartCoroutine(FindObjectOfType<GameController>().BeginRandomEvent());

            panicking = false;
            clockAnim.ResetTrigger("NearEnd");
            clockAnim.ResetTrigger("EventEnd");
            clockAnim.SetTrigger("BeginEvent");
        }
    }
}
