using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    // Duration, coded internally. Always 5s before despawn
    float duration;

    // Speed and direction
    public float moveSpeed;
    public Vector2 direction;

    // The sprite turned to face the given direction
    public Transform flippedSprite;

    private void Start()
    {
        duration = 5f;
    }

    private void Update()
    {
        duration -= Time.deltaTime;

        // Move unless duration is up
        if(duration <= 0f)
        {
            Destroy(gameObject);
        }
        else
        {
            MoveOrb();
        }
    }

    void MoveOrb()
    {
        // Simple motion
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Damage character, so long as they can move
        if (collision.gameObject.GetComponent<MainCharacter>()
            && collision.gameObject.GetComponent<MainCharacter>().canMove)
        {
            // Impact noise
            FindObjectOfType<AudioController>().Play("Impact");

            // Reduce HP
            FindObjectOfType<MainCharacter>().TakeDamage(1);

            // Destroy orb
            Destroy(gameObject);
        }
    }
}
