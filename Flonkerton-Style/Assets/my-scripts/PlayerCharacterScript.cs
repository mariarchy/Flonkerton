using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour
{
    // const string ENEMY = "Enemy";

    bool isJumping;
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
    public GameObject[] stripPrefabs;
    // TODO: rename this once characters have been changed
    public GameObject playerMesh;

    int stripIndex = 0;
    private List<GameObject> strips;
    private float midpointX;
    private bool isDead = false;
    private bool playingDeathAnimation = false;
    private float DEATH_SCALE_Z = 0.2F;
    private float DEATH_ROTATION = -90.0F;

    // Start is called before the first frame update
    void Start()
    {
        isJumping = false;
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

        if (Input.GetMouseButtonDown(0) && !isJumping)
        {
            // Commence jumping movement
            initialPosition = this.transform.position;
            isJumping = true;
            Jump();
        }

        if (isJumping)
        {
            // Move player in the x (forward) and y (jump) direction, smoothly
            // If the player has not reached the midpoint of the movement, have
            // the player jump up
            if (this.transform.position.x > midpointX)
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
                isJumping = false;
                this.transform.position = new Vector3(this.transform.position.x,
                                                      initialPosition.y,
                                                      this.transform.position.z);
            }
        }

        // If player is in the middle of a dealth animation, update next Death
        // Animation values
        if (playingDeathAnimation)
        {
            UpdateDeathAnimation();
        }
    }

    // TODO: WHY IS THE PLAYER ALWAYS STUCK NEAR THE END?
    void Jump()
    {
        // Iterate to the next strip on the map
        stripIndex += 1;
        GameObject currStrip = strips[stripIndex] as GameObject;

        // Set the end position of the jump as the next strip we want to jump to
        endPosition = new Vector3(
            currStrip.transform.position.x - POS_OFFSET,
            currStrip.transform.position.y,
            currStrip.transform.position.z
        );
        // Set midpoint/turning point for the arc of the player's jump
        midpointX = ((currStrip.transform.position.x - POS_OFFSET) +
                      this.transform.position.x) / 2;

        SpawnNewStrip();
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
        if (other.gameObject.tag == "Enemy")
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

        Debug.Log(playerMesh.transform.rotation.eulerAngles.x);

        if (playerMesh.transform.rotation.eulerAngles.x == 0 || playerMesh.transform.rotation.eulerAngles.x > 270)
        {
            Debug.Log("ROTATED");
            playerMesh.transform.Rotate(DEATH_ROTATION, 0, 0);
        }
    }
}
