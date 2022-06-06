using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    
    public float moveSpeed;

    public Animator bodyAnim;
    public ParticleSystem bloodParticles;

    public bool canMove;

    public float invulnerableTimer; // prevents multiple hits in one minigame

    public GameObject forcefield; // bubble when cheating

    // Invulnerability activated by pressing Z 10 times within 10 seconds
    bool isCheating;
    int timesButtonPressed;

    private void Start()
    {
        canMove = true;

        currentHP = maxHP;

        // Update slider
        TakeDamage(-100);
    }

    private void Update()
    {
        if (canMove)
        {
            MoveCharacter();
        }

        if(invulnerableTimer > 0f)
        {
            invulnerableTimer -= Time.deltaTime;
        }

        // Check for cheats if not cheating already
        if(!isCheating)
        {
            CheckCheat();
        }
    }

    void CheckCheat()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            timesButtonPressed++;

            if (timesButtonPressed > 49)
            {
                isCheating = true;
                forcefield.SetActive(true);
            }
        }
    }

    void MoveCharacter()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // When the game isn't corrupted yet, you can only move right.
        if (!FindObjectOfType<GameController>().gameCorrupted)
        {
            inputY = 0f;

            // No moving left past a certain point
            if(inputX < 0f && transform.position.x < -5f)
            {
                inputX = 0f;
            }
        }

        // Check boundaries
        if ((transform.position.x > 8f && inputX > 0f)
            || (transform.position.x < -8f && inputX < 0f))
        {
            inputX = 0f;
        }

        if ((transform.position.y > 3.5f && inputY > 0f)
            || (transform.position.y < -5f && inputY < 0f))
        {
            inputY = 0f;
        }

        bodyAnim.SetBool("Walking", (inputX != 0f || inputY != 0f));

        // Simple movement
        transform.position = new Vector3(transform.position.x + inputX * moveSpeed * Time.deltaTime,
                                        transform.position.y + inputY * moveSpeed * Time.deltaTime,
                                        0f);
    }

    public void TakeDamage(int value)
    {
        // Take damage
        // When cheating, you can only be healed
        if(value < 0 || !isCheating)
        {
            currentHP -= value;
        }

        if(currentHP < 0)
        {
            currentHP = 0;
        }
        else if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        // Update slider
        FindObjectOfType<GameController>().healthSlider.value = (float)currentHP / maxHP;
        FindObjectOfType<GameController>().damageBar.color = new Color32(255, 255, 255, (byte)(50 * (4 - currentHP)));

        // When taking damage, play blood particles
        // Does not work if cheating
        if (value >= 0)
        {
            FindObjectOfType<Camera>().GetComponent<Animator>().SetTrigger("Shake");
            FindObjectOfType<AudioController>().Play("Impact");
            bodyAnim.SetTrigger("Hurt");

            // Brief invincibility
            invulnerableTimer = 0.25f;

            bloodParticles.Clear();
            bloodParticles.Play();
        }

        if(currentHP == 0)
        {
            FindObjectOfType<GameController>().CharacterDeath();
        }
    }
}
