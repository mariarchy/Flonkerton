using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    const int SPEED = 10;
    float LERP_TIME = 1;         // Duration of the jump animation

    // DIRECTION VECTORS
    Vector3 UP = new Vector3(0, 0, 1);
    Vector3 DOWN = new Vector3(0, 0, -1);
    Vector3 RIGHT = new Vector3(1, 0, 0);
    Vector3 LEFT = new Vector3(-1, 0, 0);

    float currentLerpTime;
    float percent = 1;

    Vector3 startPos;
    Vector3 endPos;

    public bool jumped;

    // Update is called once per frame
    void Update()
    {
        // Get player's arrow keys
        if (Input.GetButtonDown("up") || Input.GetButtonDown("down") ||
            Input.GetButtonDown("left") || Input.GetButtonDown("right"))
        {
            if (percent == 1)     // If player moved, initialize jump start vars
            {
                currentLerpTime = 0;
                jumped = true;
            }
        }

        startPos = gameObject.transform.position;   // Update start position

        // MOVE RIGHT: Update position
        if (Input.GetButtonDown("right") &&
            gameObject.transform.position == endPos)
        {
            endPos += RIGHT;
        }

        // MOVE LEFT: Update position
        if (Input.GetButtonDown("left") &&
            gameObject.transform.position == endPos)
        {
          endPos += LEFT;
        }

        // MOVE UP: Update position
        if (Input.GetButtonDown("up") &&
            gameObject.transform.position == endPos)
        {
            endPos += UP;
        }

        // MOVE UP: Update position
        if (Input.GetButtonDown("down") &&
            gameObject.transform.position == endPos)
        {
            endPos += DOWN;
        }

        currentLerpTime += Time.deltaTime * SPEED;      // Update
        percent = currentLerpTime / LERP_TIME;

        // Interpolate position from start position & end position
        // using the percentage parametric
        gameObject.transform.position = Vector3.Lerp(startPos,
                                                     endPos,
                                                     percent);

        // Since the speed is high, immediately round values > 0.9 to 1 to
        // signal the completion of the jump animation.
        if (percent > 0.9)
        {
          percent = 1;
        }

        // If jump is complete, stop animation
        if (Mathf.Round(percent) == 1)
        {
          jumped = false;
        }
    }
}
