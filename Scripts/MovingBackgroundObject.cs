using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackgroundObject : MonoBehaviour
{
    public bool horizontalMotion;

    public float x_range;
    public float y_range;

    private float chosen_x;
    private float chosen_y;

    public float speedBase;
    public float speedVariation; // % potential variation in speed
    private float currentSpeed;

    private void Start()
    {
        SelectChosenPosition();
    }

    void Update()
    {
        MoveObject();
    }

    void SelectChosenPosition()
    {
        currentSpeed = Random.Range(speedBase - speedVariation, speedBase + speedVariation);

        chosen_x = Random.Range(-x_range, x_range);
        chosen_y = Random.Range(-y_range, y_range);
    }

    void MoveObject()
    {
        if (horizontalMotion && Mathf.Abs(transform.position.x - chosen_x) > 0.75f)
        {
            if(transform.position.x > chosen_x)
            {
                transform.position = new Vector3(transform.position.x - (currentSpeed * Time.deltaTime), transform.position.y, 0f);
            }
            else
            {
                transform.position = new Vector3(transform.position.x + (currentSpeed * Time.deltaTime), transform.position.y, 0f);
            }
        }
        else if(!horizontalMotion && Mathf.Abs(transform.position.y - chosen_y) > 0.75f) // moves vertically
        {
            if (transform.position.y > chosen_y)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - (currentSpeed * Time.deltaTime), 0f);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + (currentSpeed * Time.deltaTime), 0f);
            }
        }
        else
        {
            SelectChosenPosition();
        }
    }
}
