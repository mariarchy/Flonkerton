using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerCharacterScript : MonoBehaviour
{
    public MapController mapController;

    // GAME CHARACTER OPTIONS
    public GameObject char1;
    public GameObject char2;
    public GameObject char3;
    public GameObject char4;

    // GAME MAP VARIABLES
    Vector3 startPosition;
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
    public GameObject strip13;

    public GameObject strip_office1;
    public GameObject strip_office2;
    public GameObject strip_office3;

    public GameObject strip_outside_wall;
    public GameObject strip_office_wall;
    //public GameObject strip_outside_empty;
    //public GameObject strip_office_empty;

    // PLAYER MOVEMENT VARIABLES
    bool isJumpingUp;
    bool isJumpingDown;
    bool isJumpingRight;
    bool isJumpingLeft;

    public float POS_OFFSET = 2.3F;
    public float SPEED = 40;
    public float JUMP_INCREMENT = 40F;
    public float HORIZONTAL_JUMP_DISTANCE = 7.0F;
    public GameObject boundaryLeft;
    public GameObject boundaryRight;
    public GameObject[] stripOutsidePrefabs;
    public GameObject[] stripOfficePrefabs;

    // TODO: rename this once characters have been changed
    public GameObject playerMesh;

    // Vectors used to rotate player character in different directions
    private Vector3 FRONT = new Vector3(0, 0, 0);
    private Vector3 BACK = new Vector3(0, 180, 0);
    private Vector3 LEFT = new Vector3(0, 270, 0);
    private Vector3 RIGHT = new Vector3(0, 90, 0);

    // GAME MENU VARIABLES
    bool gameStarted = false;
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public GameObject characterMenuPanel;
    const string SCENE = "Flonkerton_Scene";
    const string REPLAY_SCENE = "Flonkerton_Scene";
    public GameObject ghost;

    // GAME OBJECT TAGS
    const string ENEMY = "Enemy";
    const string OBSTACLE = "Obstacle";
    const string COIN = "Coin";

    // SCORE AND SCHRUTE BUCKS VARIABLES
    public int score = 0;
    public int schruteBucks = 0;
    public Text scoreText;
    public Text schruteBucksText;
    int stripIndex = 0;
    private int furthestStrip = 0;

    // AUDIO VARIABLES
    public AudioClip coinClip;
    public AudioClip introClip;
    AudioSource audio;

    // MAP VARIABLES
    private List<GameObject> strips;
    private bool isOffice = false;

    // DEATH ANIMATION VARIABLES
    private float midpoint;
    private bool isDead = false;
    private bool playingDeathAnimation = false;
    private float DEATH_SCALE_Z = 0.2F;
    private float DEATH_ROTATION = -90.0F;

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
        strips.Add(strip13);

        HideGameOverPanel();

        startPanel.SetActive(true);

        if (!PlayerPrefs.HasKey("selectedChar")) {
          PlayerPrefs.SetInt("selectedChar", 0);    // Select default character
        }
        setSelectedCharacter();                     // Activate selected char

        PlayerPrefs.SetInt("play", 0);            // Disable game
        // Check if player is reloading the game or just starting it
        // Return the current Active Scene in order to get the current Scene name.
        Scene curr_scene = SceneManager.GetActiveScene();
        if (curr_scene.name == REPLAY_SCENE) {
            startPanel.SetActive(false);
            StartButtonPressed();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted) {
          return;
        }
        // Terminate if the player is dead
        if (isDead)
        {
            return;
        }


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
                                                      startPosition.y,
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
                                                      startPosition.y,
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
                this.transform.position = new Vector3(this.transform.position.x,
                                                      startPosition.y,
                                                      this.transform.position.z);
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
                this.transform.position = new Vector3(this.transform.position.x,
                                                      startPosition.y,
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

    //void SpawnNewStrip()
    //{
    //     Generate a random strip type from list of unique strip types
    //    int randStrip;
    //    GameObject stripType;

    //    if (!isOffice)
    //    {
    //        randStrip = Random.Range(0, stripOutsidePrefabs.Length);
    //        stripType = stripOutsidePrefabs[randStrip] as GameObject;
    //    }
    //    else {
    //        randStrip = Random.Range(0, stripOfficePrefabs.Length);
    //        stripType = stripOfficePrefabs[randStrip] as GameObject;
    //    }
        

    //     Retrieve width of strip by accessing its grandchild's transform
    //    Transform childTransform = stripType.transform.GetChild(0) as Transform;
    //    Transform grandchildTransform = childTransform.GetChild(0) as Transform;
    //     Get the x coordinate (width) of the strip's mesh box
    //    float width = grandchildTransform.gameObject.GetComponent<Renderer>()
    //                  .bounds.size.x;
    //    Debug.Log(width);


    //     Use last strip coordinates to instantiate new strip object
    //    GameObject lastStrip = strips[strips.Count - 1] as GameObject;
    //     Clone last strip to instantiate new strip object
    //    GameObject newStrip = Instantiate(stripType,
    //                                      lastStrip.transform.position,
    //                                      lastStrip.transform.rotation);
    //     Set new  strip to the next available slot in map
    //    newStrip.transform.position = new Vector3(
    //        newStrip.transform.position.x - width,
    //        stripType.transform.position.y,
    //        stripType.transform.position.z);

    //     Add strip to map
    //    strips.Add(newStrip);
    //}

    // Checks for enemy collisions
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ENEMY)
        {
            Debug.Log(other.gameObject.name);
            DeathAnimation();
        }

        if (other.gameObject.tag == OBSTACLE)
        {
            Debug.Log("Collision with obstacle has occurred");
            // Reset values using offset so player stays in place
            this.transform.position = new Vector3(
                this.transform.position.x,
                startPosition.y,
                this.transform.position.z);

            isJumpingUp = isJumpingDown = isJumpingLeft = isJumpingRight = false;
        }

	if (other.gameObject.tag == COIN)
	{
	    Debug.Log("Collision with coin");
	    // Update Schrute Bucks count
	    schruteBucks += 1;
            schruteBucksText.text = "Schrute Bucks: " + schruteBucks.ToString();

	    // Play coin sound
	    this.GetComponent<AudioSource>().PlayOneShot(coinClip);
	    // Destroy coin so player cannot collect it again
	    Destroy(other.gameObject);
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
            // Game Over
            DisplayGameOverPanel();
        }

        float playerRotationX = playerMesh.transform.eulerAngles.x;

        if (playerMesh.transform.rotation.eulerAngles.x == 0 || playerMesh.transform.rotation.eulerAngles.x > 270)
        {
            playerMesh.transform.Rotate(DEATH_ROTATION, 0, 0);
        }
    }

    void SwipeUp()
    {
        //Debug.Log("Consuming swipe up");
        if (gameStarted && !isJumpingUp)
        {
            isJumpingUp = true;
            JumpUp();
        }
    }

    void SwipeDown()
    {
        //Debug.Log("Consuming swipe down");
        if (gameStarted && !isJumpingDown)
        {
            isJumpingDown = true;
            JumpDown();
        }
    }

    void SwipeRight()
    {
        //Debug.Log("Consuming swipe right");
        if (gameStarted && !isJumpingRight)
        {
            isJumpingRight = true;
            JumpRight();
        }
    }

    void SwipeLeft()
    {
        //Debug.Log("Consuming swipe left");
        if (gameStarted && !isJumpingLeft)
        {
            isJumpingLeft = true;
            JumpLeft();
        }
    }


    // TODO: WHY IS THE PLAYER ALWAYS STUCK NEAR THE END?
    void JumpUp()
    {
        // Iterate to the next strip on the map
        stripIndex++;
        // Update score only if player is jumping to a "higher level" strip
        if (stripIndex > furthestStrip)
        {
          score++;
          scoreText.text = "Score: " + score;
          furthestStrip = stripIndex;
          //Debug.Log("Score is " + score);
        }

        GameObject nextStrip = mapController.GetStripAtIndex(stripIndex);

        // Set the start position and end position of the jump as the next strip
        // we want to jump to
        startPosition = this.transform.position;
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

        mapController.SpawnNewStrip();

        // Calculate distance travelled in jump
        moveBoundaryX(endPosition.x - this.transform.position.x);

        this.transform.position = new Vector3(this.transform.position.x,
                                              startPosition.y,
                                              this.transform.position.z);
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
            stripIndex--;
        }

        GameObject prevStrip = mapController.GetStripAtIndex(stripIndex);

        // Set the start position and end position of the jump as the next strip
        // we want to jump to
        startPosition = this.transform.position;
        endPosition = new Vector3(
            prevStrip.transform.position.x - POS_OFFSET,
            this.transform.position.y,
            this.transform.position.z
        );
        // Set midpoint/turning point for the arc of the player's jump
        midpoint = (endPosition.x + this.transform.position.x) / 2;

        // Set character to face the front
        playerMesh.transform.localEulerAngles = BACK;

        // Calculate distance travelled in jump
        moveBoundaryX(endPosition.x - this.transform.position.x);
    }

    void JumpRight()
    {
        // Set the start position and end position of the jump as the next strip
        // we want to jump to
        startPosition = this.transform.position;
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
        // Set the start position and end position of the jump as the next strip
        // we want to jump to
        startPosition = this.transform.position;
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

    // Move left and right boundaries when player jumps up or down
    void moveBoundaryX(float distance)
    {
        // Calculate distance travelled in jump
        boundaryLeft.transform.position += new Vector3(distance, 0, 0);
        boundaryRight.transform.position += new Vector3(distance, 0, 0);
    }

    // Start the game and hide the start panel when button is pressed
    void StartButtonPressed() {
        Debug.Log("Start Button Pressed");
        gameStarted = true;
        PlayerPrefs.SetInt("play", 1);
        startPanel.SetActive(false);

        // Stop intro song audio
        AudioSource intro = this.GetComponents<AudioSource>()[1];
        if (intro.isPlaying) {
            intro.Stop();
        }
    }

    // CHARACTER MENU LISTENERS
    // Load character menu
    void CharacterMenuButtonPressed() {
        Debug.Log("Character Menu Button Pressed");
        // Hide start menu panel & display menu panel
        characterMenuPanel.SetActive(true);
    }

    // Load char1
    void CharacterSelectedChar1() {
        Debug.Log("Character 1 Selected");
        PlayerPrefs.SetInt("selectedCharacter", 0);
        // Return to main menu after selection
        characterMenuPanel.SetActive(false);
        setSelectedCharacter();
    }

    // Load char2
    void CharacterSelectedChar2() {
        Debug.Log("Character 2 Selected");
        PlayerPrefs.SetInt("selectedCharacter", 1);
        // Return to main menu after selection
        characterMenuPanel.SetActive(false);
        setSelectedCharacter();
    }

    // Load char3
    void CharacterSelectedChar3() {
        Debug.Log("Character 3 Selected");
        PlayerPrefs.SetInt("selectedCharacter", 2);
        // Return to main menu after selection
        characterMenuPanel.SetActive(false);
        setSelectedCharacter();
    }

    // Load char4
    void CharacterSelectedChar4() {
        Debug.Log("Character 4 Selected");
        PlayerPrefs.SetInt("selectedCharacter", 3);
        // Return to main menu after selection
        characterMenuPanel.SetActive(false);
        setSelectedCharacter();
    }

    // Set active character based on user input in the character menu
    void setSelectedCharacter() {
        int selectedChar = PlayerPrefs.GetInt("selectedCharacter");
        char1.SetActive(false);
        char2.SetActive(false);
        char3.SetActive(false);
        char4.SetActive(false);
        switch(selectedChar) {
          case 0:
            char1.SetActive(true);
            playerMesh = char1;
            break;
          case 1:
            char2.SetActive(true);
            playerMesh = char2;
            break;
          case 2:
            char3.SetActive(true);
            playerMesh = char3;
            break;
          case 3:
            char4.SetActive(true);
            playerMesh = char4;
            break;
        }
    }

    void DisplayGameOverPanel() {
        gameStarted = false;
        gameOverPanel.SetActive(true);
    }

    void HideGameOverPanel() {
        gameOverPanel.SetActive(false);
    }

    void PlayAgain() {
        Debug.Log("Play again button pressed");

        // Reset level and start over
        SceneManager.LoadScene(REPLAY_SCENE);
    }
}
