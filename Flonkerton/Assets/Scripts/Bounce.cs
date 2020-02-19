using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    float lerpTime;         // Controls lerp time
    float currentLerpTime;
    float percent = 1;      // Completion percentage

    Vector3 startPos;
    Vector3 endPos;

    bool firstInput;        // Check if first input has been made
    public bool jumped;

    // Update is called once per frame
    void Update()
    {
        // Get player's arrow keys
        if (Input.GetButtonDown("up") || Input.GetButtonDown("down") ||
            Input.GetButtonDown("left") || Input.GetButtonDown("right"))
        {
            if (percent == 1)     // Initialize start variables
            {
                lerpTime = 1;
                currentLerpTime = 0;
                firstInput = true;
                jumped = true;
            }
        }

        startPos = gameObject.transform.position;

        // MOVE RIGHT: Update position
        if (Input.GetButtonDown("right") &&
            gameObject.transform.position == endPos)
        {
            endPos = new Vector3(transform.position.x + 1,
                                 transform.position.y,
                                 transform.position.z);
        }

        // MOVE RIGHT: Update position
        if (Input.GetButtonDown("right") &&
            gameObject.transform.position == endPos)
        {
            endPos = new Vector3(transform.position.x + 1,
                                 transform.position.y,
                                 transform.position.z);
        }

        // MOVE LEFT: Update position
        if (Input.GetButtonDown("left") &&
            gameObject.transform.position == endPos)
        {
          endPos = new Vector3(transform.position.x - 1,
                               transform.position.y,
                               transform.position.z);
        }

        // MOVE UP: Update position
        if (Input.GetButtonDown("up") &&
            gameObject.transform.position == endPos)
        {
            endPos = new Vector3(transform.position.x,
                                 transform.position.y,
                                 transform.position.z + 1);
        }

        // MOVE UP: Update position
        if (Input.GetButtonDown("down") &&
            gameObject.transform.position == endPos)
        {
            endPos = new Vector3(transform.position.x,
                                 transform.position.y,
                                 transform.position.z - 1);
        }

        if (firstInput)
        {
            currentLerpTime += Time.deltaTime * 5;      // Update by FPS
            percent = currentLerpTime / lerpTime;
            // Lerp from start position to end position by the percentage
            gameObject.transform.position = Vector3.Lerp(startPos,
                                                         endPos,
                                                         percent);

            if (percent > 0.8)
            {
              percent = 1;
            }

            if (Mathf.Round(percent) == 1)
            {
              jumped = false;
            }
        }
    }
}
