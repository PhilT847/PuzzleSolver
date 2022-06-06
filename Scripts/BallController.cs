using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public Slider ballSlider;
    public int ballCount;

    public Transform ballGoal;

    public GameObject[] allBalls;

    private void Start()
    {
        ballCount = 0;
        ballSlider.value = 0;

        // Create first ball
        Instantiate(allBalls[0], Vector3.zero, Quaternion.identity);
        ballGoal.transform.position = new Vector3(4.5f, 0f, 0f);
    }

    public void AddBall()
    {
        // Play star particles
        ballGoal.GetComponentInChildren<ParticleSystem>().Clear();
        ballGoal.GetComponentInChildren<ParticleSystem>().Play();

        // First ball corrupts game
        if(ballCount == 0)
        {
            StartCoroutine(InitiateCorruption());
        }
        else // Otherwise, just destroy it
        {
            FindObjectOfType<AudioController>().Play("BallGoal");
            Destroy(FindObjectOfType<Ball>().gameObject);
        }

        ballCount++;

        ballSlider.value = ballCount / 8f;

        // Change skull spawner level
        FindObjectOfType<DamageOrbSpawner>().DetermineLevel(ballCount);

        if (ballCount >= 8)
        {
            ballCount = 8;

            FindObjectOfType<GameController>().StartCoroutine(FindObjectOfType<GameController>().WinGame());
        }
        else
        {
            // Create new ball if not finished
            Instantiate(allBalls[ballCount], Vector3.zero, Quaternion.identity);

            int spawnSide = Random.Range(0, 2);

            // Right side
            if(spawnSide == 0)
            {
                // Move ball goal and scale to flip
                ballGoal.position = new Vector3(Random.Range(3f, 7f), Random.Range(-3f, 3f), 0f);
                ballGoal.localScale = new Vector3(1f, 1f, 1f);
            }
            else // Left side
            {
                // Move ball goal and scale to flip
                ballGoal.position = new Vector3(Random.Range(-3f, -7f), Random.Range(-3f, 3f), 0f);
                ballGoal.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }

    public IEnumerator InitiateCorruption()
    {
        FindObjectOfType<MainCharacter>().canMove = false;

        Ball starterBall = FindObjectOfType<Ball>();

        FindObjectOfType<AudioController>().Play("DeepWhoosh");

        starterBall.ballAnim.SetTrigger("Corrupt");

        yield return new WaitForSeconds(0.52f);

        FindObjectOfType<GameController>().CorruptGame();

        yield return new WaitForSeconds(0.5f);

        Destroy(starterBall.gameObject);

        FindObjectOfType<MainCharacter>().canMove = true;

        yield return null;
    }
}
