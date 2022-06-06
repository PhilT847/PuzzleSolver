using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrennanObject : MonoBehaviour
{
    public MainCharacter theCharacter;

    public SpriteRenderer faceSprite;
    public Sprite[] proximitySprites;

    public bool checkingProximity;

    private void Start()
    {
        StartCoroutine(InitializeBrennan());
    }

    void Update()
    {
        if (checkingProximity)
        {
            CheckProximity();
        }
    }

    public IEnumerator InitializeBrennan()
    {
        theCharacter = FindObjectOfType<MainCharacter>();

        transform.position = new Vector3(Random.Range(-8f,8f), Random.Range(-4f,4f), 0f);

        checkingProximity = true;

        yield return null;
    }

    void CheckProximity()
    {
        float distance = Vector2.Distance(transform.position, theCharacter.transform.position);

        // Change sprite based on distance. Explode if too close.
        if (distance > 9f)
        {
            faceSprite.sprite = proximitySprites[0];
        }
        else if (distance > 6f)
        {
            faceSprite.sprite = proximitySprites[1];
        }
        else if (distance > 3f)
        {
            faceSprite.sprite = proximitySprites[2];
        }
        else
        {
            faceSprite.sprite = proximitySprites[3];

            StartCoroutine(Explode());

            return;
        }
    }

    public IEnumerator Explode()
    {
        checkingProximity = false;

        // Grant points
        if (FindObjectOfType<Event>())
        {
            FindObjectOfType<Event>().points++;
        }

        int animRoll = Random.Range(1, 5);

        if(animRoll < 4)
        {
            GetComponentInChildren<Animator>().SetTrigger("Explode1");
        }
        else
        {
            GetComponentInChildren<Animator>().SetTrigger("Explode2");
        }

        FindObjectOfType<AudioController>().Play("ExplodeGore");

        yield return new WaitForSeconds(0.23f);

        GetComponentInChildren<ParticleSystem>().Clear();
        GetComponentInChildren<ParticleSystem>().Play();

        if (FindObjectOfType<Event>() 
            && FindObjectOfType<Event>().points >= FindObjectOfType<Event>().pointsNeeded)
        {
            /*
            FindObjectOfType<GameController>().WinEvent();

            Destroy(FindObjectOfType<Event>().gameObject);
            */

            // Force end of event
            FindObjectOfType<Event>().eventDuration = 0.02f;
        }

        Destroy(gameObject, 1.5f);

        yield return null;
    }
}
