using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float ballSpeed;

    public float noPersonContact; // 1s between ball kicking from player

    public Vector3 currentMoveVector;

    public Animator ballAnim;

    private void Update()
    {
        if(noPersonContact > 0f)
        {
            noPersonContact -= Time.deltaTime;
        }

        // Look for walls while rolling
        if (transform.position.y > 4f || transform.position.y < -4f
            || transform.position.x > 8f || transform.position.x < -8f)
        {
            HitWall();
        }
    }

    public IEnumerator PushBall(Vector3 contactPoint, float speed)
    {
        FindObjectOfType<AudioController>().Play("BallHit");

        ballSpeed = speed;
        currentMoveVector = contactPoint;
        ballAnim.SetTrigger("Kick");

        while (ballSpeed > 0.2f)
        {
            ballSpeed -= 3f * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, contactPoint, -ballSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    void HitWall()
    {
        bool touchingTop = (transform.position.y > 4f);
        bool touchingBottom = (transform.position.y < -4f);
        bool touchingLeft = (transform.position.x < -8f);
        bool touchingRight = (transform.position.x > 8f);

        // Create an invisible vector around the ball if it hit a wall
        Vector3 bounceVector = new Vector3(transform.position.x, transform.position.y, 0f);

        if (touchingTop)
        {
            bounceVector = new Vector3(bounceVector.x, transform.position.y + 1f, 0f);
        }
        else if(touchingBottom)
        {
            bounceVector = new Vector3(bounceVector.x, transform.position.y - 1f, 0f);
        }

        if (touchingLeft)
        {
            bounceVector = new Vector3(transform.position.x - 1f, bounceVector.y, 0f);
        }
        else if (touchingRight)
        {
            bounceVector = new Vector3(transform.position.x + 1f, bounceVector.y, 0f);
        }

        // Add upwards or downwards motion on balls not touching both walls
        if (!touchingTop && !touchingBottom)
        {
            bounceVector = new Vector3(bounceVector.x, currentMoveVector.y, 0f);
        }
        else if (!touchingLeft && !touchingRight)
        {
            bounceVector = new Vector3(currentMoveVector.x, bounceVector.y, 0f);
        }

        // Ball bounces if it hit any walls
        if(bounceVector != Vector3.zero)
        {
            if (ballSpeed < 0.5f)
            {
                ballSpeed = 0.5f;
            }

            // Prevent player from constant bounces against the wall
            noPersonContact = 0.25f;

            StopAllCoroutines();
            StartCoroutine(PushBall(bounceVector, ballSpeed));
        }
    }

    // Hit another ball or the player
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Main character pushes Ball at a constant speed
        if (collision.gameObject.GetComponent<MainCharacter>()
            && noPersonContact <= 0f)
        {
            noPersonContact = 0.25f;

            StopAllCoroutines();
            Vector3 characterPos = new Vector3(collision.transform.position.x, collision.transform.position.y + 0.5f, 0f);
            StartCoroutine(PushBall(characterPos, 8f));
        }
        else if (collision.gameObject.GetComponent<Ball>()
                && collision.gameObject.GetComponent<Ball>().ballSpeed > 0.2f) // Balls can push balls based on their current speed
        {
            StopAllCoroutines();
            StartCoroutine(PushBall(collision.transform.position, collision.gameObject.GetComponent<Ball>().ballSpeed));
        }
        else if (collision.gameObject.layer == 9) // Goal
        {
            // Ball is destroyed by AddBall
            FindObjectOfType<BallController>().AddBall();
        }
    }
}
