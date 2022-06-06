using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float spawnPoint_X;
    public float spawnPoint_Y;

    public float spawnRange_X; // variation (+ or -)
    public float spawnRange_Y; // variation (+ or -)

    public float hurtBeginTime;
    public float hurtDuration;

    public bool automaticallyLoseOnHit;
    public Collider2D hurtbox;

    // Particles when hurtbox begins
    public ParticleSystem hurtParticles;
    public GameObject destroyedOnHurtboxActivated;

    public float waitBeforeMoving; // Wait time (Seconds) before moving
    private float moveWaitRemaining;
    public float moveSpeed_X;
    public float moveSpeed_Y;

    public string soundOnEntry; // Sound when spawned
    public string soundOnDamage; // Sound when hurtbox appears
    // All hazards play "Impact" on hit as well

    private void Start()
    {
        IntializeHazard();
    }

    void IntializeHazard()
    {
        hurtbox.enabled = false;

        transform.position = new Vector3(spawnPoint_X + Random.Range(-spawnRange_X, spawnRange_X),
                                        spawnPoint_Y + Random.Range(-spawnRange_Y, spawnRange_Y), 
                                        0f);

        moveWaitRemaining = waitBeforeMoving;

        if(soundOnEntry != "")
        {
            FindObjectOfType<AudioController>().Play(soundOnEntry);
        }

        StartCoroutine(RunHazard());
    }

    public IEnumerator RunHazard()
    {
        float waitBeforeHurt = 0f;

        while(waitBeforeHurt < hurtBeginTime)
        {
            waitBeforeHurt += Time.deltaTime;

            MoveObject();

            yield return new WaitForEndOfFrame();
        }

        // Play sound for when damage period begins
        if (soundOnDamage != "")
        {
            FindObjectOfType<AudioController>().Play(soundOnDamage);
        }

        // Activate hurtbox
        hurtbox.enabled = true;

        if(hurtParticles != null)
        {
            ParticleSystem copiedParticles = Instantiate(hurtParticles, transform.position, Quaternion.identity);

            copiedParticles.Clear();
            copiedParticles.Play();

            Destroy(copiedParticles.gameObject, 1f);
        }

        if(destroyedOnHurtboxActivated != null)
        {
            destroyedOnHurtboxActivated.SetActive(false);
        }

        while (hurtDuration > 0f)
        {
            hurtDuration -= Time.deltaTime;

            MoveObject();

            yield return new WaitForEndOfFrame();
        }

        // Remove hurtbox and destroy

        Destroy(gameObject);

        yield return null;
    }

    void MoveObject()
    {
        if(moveWaitRemaining > 0f)
        {
            moveWaitRemaining -= Time.deltaTime;
        }
        else
        {
            // Simple movement
            transform.position = new Vector3(transform.position.x + moveSpeed_X * Time.deltaTime,
                                            transform.position.y + moveSpeed_Y * Time.deltaTime,
                                            0f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MainCharacter>())
        {
            // Impact noise
            FindObjectOfType<AudioController>().Play("Impact");

            // Reduce points
            if (FindObjectOfType<Event>())
            {
                FindObjectOfType<Event>().points--;
            }

            if (FindObjectOfType<Event>() && automaticallyLoseOnHit)
            {
                /*
                FindObjectOfType<GameController>().LoseEvent();

                Destroy(FindObjectOfType<Event>().gameObject);
                */

                // Force end event
                FindObjectOfType<Event>().eventDuration = 0.02f;
            }

            // Disable hurtbox
            hurtbox.enabled = false;
        }
    }
}
