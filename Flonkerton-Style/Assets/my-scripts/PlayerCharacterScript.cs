using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour
{
    const string ENEMY = "Enemy";
    bool isJumpingUp;
    bool isJumpingDown;
    bool isJumpingRight;
    bool isJumpingLeft;

    Vector3 initialPosition;
    Vector3 endPosition;
    public GameObject strip1;
    public GameObject strip2;
    public GameObject strip3;
    public GameObject strip4;
    public GameObject strip5;
    public GameObject strip6;
    public GameObject strip7;
    public GameObject strip8;
    public GameObject strip9;
    public GameObject strip10;
    public GameObject strip11;
    public GameObject strip12;

    public float POS_OFFSET = 2.3F;
    public float SPEED = 30;
    public float JUMP_INCREMENT = 15F;
    public float HORIZONTAL_JUMP_DISTANCE = 7.0F;
    public GameObject[] stripPrefabs;
    // TODO: rename this once characters have been changed
    public GameObject playerMesh;

    int stripIndex = 0;
    private List<GameObject> strips;
    private float midpoint;
    private bool isDead = false;
    private bool playingDeathAnimation = false;
    private float DEATH_SCALE_Z = 0.2F;
    private float DEATH_ROTATION = -90.0F;

    // Vectors used to rotate player character in different directions
    private Vector3 FRONT = new Vector3(0, 0, 0);
    private Vector3 BACK = new Vector3(0, 180, 0);
    private Vector3 LEFT = new Vector3(0, 270, 0);
    private Vector3 RIGHT = new Vector3(0, 90, 0);

    // Start is called before the first frame update
    void Start()
    {
        isJumpingUp = isJumpingDown = isJumpingLeft = isJumpingRight = false;
        strips = new List<GameObject>();

        // Add all strips to List
        strips.Add(strip1);
        strips.Add(strip2);
        strips.Add(strip3);
        strips.Add(strip4);
        strips.Add(strip5);
        strips.Add(strip6);
        strips.Add(strip7);
        strips.Add(strip8);
        strips.Add(strip9);
        strips.Add(strip10);
        strips.Add(strip11);
        strips.Add(strip12);
    }

    // Update is called once per frame
    void Update()
    {
        // Terminate if the player is dead
        if (isDead)
        {
            return;
        }

        // if (Input.GetMouseButtonDown(0) && !isJumping)
        // {
        //     // Commence jumping movement
        //     initialPosition = this.transform.position;
        //     isJumping = true;
        //     Jump();
        // }

        // Update player coordinates for each jump type
        if (isJumpingUp)
        {
            // Move player in the x (forward) and y (jump) direction, smoothly
            // If the player has not reached the midpoint of the movement, have
            // the player jump up
            if (this.transform.position.x > midpoint)
            {
                this.transform.position = new Vector3(
                    this.transform.position.x - (Time.deltaTime * SPEED),
                    this.transform.position.y + (Time.deltaTime * JUMP_INCREMENT),
                    this.transform.position.z
                );
            }
            // If the player has reached past the midpoint of the movement, have
            // the player jump down
            else if (this.transform.position.x > endPosition.x)
            {
                this.transform.position = new Vector3(
                    this.transform.position.x - (Time.deltaTime * SPEED),
                    this.transform.position.y - (Time.deltaTime * JUMP_INCREMENT),
                    this.transform.position.z
                );
            }
            else
            {
                // Stop movement once player has reached movement destination
                isJumpingUp = false;
                this.transform.position = new Vector3(this.transform.position.x,
                                                      initialPosition.y,
                                                      this.transform.position.z);
            }
        } else if (isJumpingDown) {
            // Move player in the x (forward) and y (jump) direction, smoothly
            // If the player has not reached the midpoint of the movement, have
            // the player jump up
            if (this.transform.position.x < midpoint)
            {
                this.transform.position = new Vector3(
                    this.transform.position.x + (Time.deltaTime * SPEED),
                    this.transform.position.y + (Time.deltaTime * JUMP_INCREMENT),
                    this.transform.position.z
                );
            }
            // If the player has reached past the midpoint of the movement, have
            // the player jump down
            else if (this.transform.position.x < endPosition.x)
            {
                this.transform.position = new Vector3(
                    this.transform.position.x + (Time.deltaTime * SPEED),
                    this.transform.position.y - (Time.deltaTime * JUMP_INCREMENT),
                    this.transform.position.z
                );
            }
            else
            {
                // Stop movement once player has reached movement destination
                isJumpingDown = false;
                this.transform.position = new Vector3(this.transform.position.x,
                                                      initialPosition.y,
                                                      this.transform.position.z);
            }
        } else if (isJumpingLeft) {
            if (this.transform.position.z > midpoint)
            {
                this.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y + (Time.deltaTime * JUMP_INCREMENT),
                    this.transform.position.z - (Time.deltaTime * SPEED)
                );
            }
            // If the player has reached past the midpoint of the movement, have
            // the player jump down
            else if (this.transform.position.z > endPosition.z)
            {
                this.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y - (Time.deltaTime * JUMP_INCREMENT),
                    this.transform.position.z - (Time.deltaTime * SPEED)
                );
            }
            else
            {
                // Stop movement once player has reached movement destination
                isJumpingLeft = false;
            }
        } else if (isJumpingRight) {
            if (this.transform.position.z < midpoint)
            {
                this.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y + (Time.deltaTime * JUMP_INCREMENT),
                    this.transform.position.z + (Time.deltaTime * SPEED)
                );
            }
            // If the player has reached past the midpoint of the movement, have
            // the player jump down
            else if (this.transform.position.z < endPosition.z)
            {
                this.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y - (Time.deltaTime * JUMP_INCREMENT),
                    this.transform.position.z + (Time.deltaTime * SPEED)
                );
            }
            else
            {
                // Stop movement once player has reached movement destination
                isJumpingRight = false;
            }
        }


        // If player is in the middle of a dealth animation, update next Death
        // Animation values
        if (playingDeathAnimation)
        {
            UpdateDeathAnimation();
        }
    }

    void SpawnNewStrip()
    {
        // Generate a random strip type from list of unique strip types
        int randStrip = Random.Range(0, stripPrefabs.Length);
        GameObject stripType = stripPrefabs[randStrip] as GameObject;

        // Retrieve width of strip by accessing its grandchild's transform
        Transform childTransform = stripType.transform.GetChild(0) as Transform;
        Transform grandchildTransform = childTransform.GetChild(0) as Transform;
        // Get the x coordinate (width) of the strip's mesh box
        float width = grandchildTransform.gameObject.GetComponent<Renderer>()
                      .bounds.size.x;

        // Use last strip coordinates to instantiate new strip object
        GameObject lastStrip = strips[strips.Count - 1] as GameObject;
        // Clone last strip to instantiate new strip object
        GameObject newStrip = Instantiate(stripType,
                                          lastStrip.transform.position,
                                          lastStrip.transform.rotation);
        // Set new  strip to the next available slot in map
        newStrip.transform.position = new Vector3(
            newStrip.transform.position.x - width,
            stripType.transform.position.y,
            stripType.transform.position.z);

        // Add strip to map
        strips.Add(newStrip);
    }

    // Checks for enemy collisions
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ENEMY)
        {
            Debug.Log("Collision has occurred");
            DeathAnimation();
        }
    }

    // Trigger death animation
    void DeathAnimation()
    {
        playingDeathAnimation = true;
    }

    // Player death animation
    void UpdateDeathAnimation()
    {
        // If player mesh has not completed animation, descale and complete it
        if (playerMesh.transform.localScale.z > DEATH_SCALE_Z)
        {
            playerMesh.transform.localScale -= new Vector3(0, 0, DEATH_SCALE_Z);
        }
        else
        {
            // Animation is completed
            playingDeathAnimation = false;
            isDead = true;
        }

        float playerRotationX = playerMesh.transform.eulerAngles.x;

        if (playerMesh.transform.rotation.eulerAngles.x == 0 || playerMesh.transform.rotation.eulerAngles.x > 270)
        {
            playerMesh.transform.Rotate(DEATH_ROTATION, 0, 0);
        }
    }

    void SwipeUp()
    {
        Debug.Log("Consuming swipe up");
        if (!isJumpingUp)
        {
            isJumpingUp = true;
            JumpUp();
        }
    }

    void SwipeDown()
    {
        Debug.Log("Consuming swipe down");
        if (!isJumpingDown)
        {
            isJumpingDown = true;
            JumpDown();
        }
    }

    void SwipeRight()
    {
        Debug.Log("Consuming swipe right");
        if (!isJumpingRight)
        {
            isJumpingRight = true;
            JumpRight();
        }
    }

    void SwipeLeft()
    {
        Debug.Log("Consuming swipe left");
        if (!isJumpingLeft)
        {
            isJumpingLeft = true;
            JumpLeft();
        }
    }


    // TODO: WHY IS THE PLAYER ALWAYS STUCK NEAR THE END?
    void JumpUp()
    {
        // Iterate to the next strip on the map
        stripIndex += 1;
        GameObject nextStrip = strips[stripIndex] as GameObject;

        // Set the end position of the jump as the next strip we want to jump to
        endPosition = new Vector3(
            nextStrip.transform.position.x - POS_OFFSET,
            nextStrip.transform.position.y,
            nextStrip.transform.position.z
        );
        // Set midpoint/turning point for the arc of the player's jump
        midpoint = ((nextStrip.transform.position.x - POS_OFFSET) +
                      this.transform.position.x) / 2;

        // Set character to face the front
        playerMesh.transform.localEulerAngles = FRONT;

        SpawnNewStrip();
    }

    void JumpDown()
    {
        // Only iterate to the previous strip if player is past the first strip
        if (stripIndex == 0)
        {
            return;
        }
        else
        {
            stripIndex -= 1;
        }

        GameObject prevStrip = strips[stripIndex] as GameObject;

        // Set the end position of the jump as the next strip we want to jump to
        endPosition = new Vector3(
            prevStrip.transform.position.x - POS_OFFSET,
            this.transform.position.y,
            this.transform.position.z
        );
        // Set midpoint/turning point for the arc of the player's jump
        midpoint = (endPosition.x + this.transform.position.x) / 2;

        // Set character to face the front
        playerMesh.transform.localEulerAngles = BACK;
    }

    void JumpRight()
    {
        // Set the end position of the jump as the next strip we want to jump to
        endPosition = new Vector3(
            this.transform.position.x,
            this.transform.position.y,
            this.transform.position.z + HORIZONTAL_JUMP_DISTANCE
        );
        // Set midpoint/turning point for the arc of the player's jump
        midpoint = (this.transform.position.z + endPosition.z) / 2;

        // Set character to face the front
        playerMesh.transform.localEulerAngles = RIGHT;
    }

    void JumpLeft()
    {
        // Set the end position of the jump as the next strip we want to jump to
        endPosition = new Vector3(
            this.transform.position.x,
            this.transform.position.y,
            this.transform.position.z - HORIZONTAL_JUMP_DISTANCE
        );
        // Set midpoint/turning point for the arc of the player's jump
        midpoint = (this.transform.position.z + endPosition.z) / 2;

        // Set character to face the front
        playerMesh.transform.localEulerAngles = LEFT;
    }
}
