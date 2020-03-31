using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Timers;
using System.Globalization;

public class PlayerCharacterScript : MonoBehaviour
{
    public MapController mapController;

    // GAME CHARACTER OPTIONS
    public List<GameObject> CharList;

    // GAME MAP VARIABLES
    Vector3 startPosition;
    Vector3 endPosition;

    // PLAYER MOVEMENT VARIABLES
    bool isJumpingUp;
    bool isJumpingDown;
    bool isJumpingRight;
    bool isJumpingLeft;

    public float POS_OFFSET = 2.3F;
    public float SPEED = 40;
    public float JUMP_INCREMENT = 40F;
    public float HORIZONTAL_JUMP_DISTANCE = 10.0F;
    public GameObject boundaryLeft;
    public GameObject boundaryRight;

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
    // TODO: Update these via the Object tag
    public Text scoreText;
    public Text scoreTextOutline;
    public Text schruteBucksText;
    public Text schruteBucksTextOutline;
    public Text highScoreText;
    public Text highScoreOutline;
    public Text finalScore;
    private int schruteBucks;
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

    //PAUSE MENU VARIABLES
    public static bool isPaused = false;
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        isJumpingUp = isJumpingDown = isJumpingLeft = isJumpingRight = false;

        HideGameOverPanel();

        startPanel.SetActive(true);

        if (!PlayerPrefs.HasKey("schruteBucks")) {
          PlayerPrefs.SetInt("schruteBucks", 0);    // Set schrute bucks count
        }
        // Update Schrute Bucks value
        schruteBucks = PlayerPrefs.GetInt("schruteBucks");
        schruteBucksText.text = schruteBucks.ToString();
        schruteBucksTextOutline.text = schruteBucksText.text;

        if (!PlayerPrefs.HasKey("selectedChar")) {
          PlayerPrefs.SetInt("selectedChar", 0);    // Select default character
        }
        setSelectedCharacter();                     // Activate selected char

        PlayerPrefs.SetInt("play", 0);            // Disable game
        // Check if player is reloading the game or just starting it
        int isReloaded = PlayerPrefs.GetInt("reloaded");
        Debug.Log(isReloaded);
        if (isReloaded == 1) {
            startPanel.SetActive(false);
            StartButtonPressed();
        }

        highScoreText.text = PlayerPrefs.GetInt("highestScore").ToString();
        highScoreOutline.text = PlayerPrefs.GetInt("highestScore").ToString();
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
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

    // Checks for enemy collisions
    void OnTriggerEnter(Collider other)
    {
        // CHARACTER HITS ENEMY - GAMEOVER
        if (other.gameObject.tag == ENEMY)
        {
            Debug.Log(other.gameObject.name);

            // Store schrute bucks from game session
            PlayerPrefs.SetInt("schruteBucks", schruteBucks);

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
      schruteBucksText.text = schruteBucks.ToString();
      schruteBucksTextOutline.text = schruteBucksText.text;

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
          scoreText.text = score.ToString();
          scoreTextOutline.text = scoreText.text;
          furthestStrip = stripIndex;

          // Update high score if needed
          if (score > PlayerPrefs.GetInt("highestScore",0))
          {
              PlayerPrefs.SetInt("highestScore",score);
              // Update high score text
              highScoreText.text = score.ToString() + " !";
              highScoreOutline.text = highScoreText.text + " !";
          }
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
        // Update Schrute bucks count
        schruteBucks = PlayerPrefs.GetInt("schruteBucks");
        schruteBucksText.text = schruteBucks.ToString();
        schruteBucksTextOutline.text = schruteBucksText.text;
        gameStarted = true;
        PlayerPrefs.SetInt("play", 1);
        PlayerPrefs.SetInt("reloaded", 0);
        setSelectedCharacter();
        startPanel.SetActive(false);
        HideGameOverPanel();

        // Stop intro song audio
        AudioSource intro = this.GetComponents<AudioSource>()[1];
        if (intro.isPlaying) {
            intro.Stop();
        }
    }

    void Resume()
    {
        Debug.Log("Resuming");
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void TutorialButtonPressed() {
        Debug.Log("Tutorial Button Pressed");
        SceneManager.LoadScene("Tutorial_Scene");
    }

    // CHARACTER MENU LISTENERS
    // Set active character based on user input in the character menu
    void setSelectedCharacter() {
        int selectedChar = PlayerPrefs.GetInt("selectedChar");
        foreach(var character in CharList) {
          character.SetActive(false);
        }
        Debug.Log(CharList.Count);
        GameObject selectedCharacter = CharList[selectedChar];
        selectedCharacter.SetActive(true);
        playerMesh = selectedCharacter;
        // char1.SetActive(false);
        // char2.SetActive(false);
        // char3.SetActive(false);
        // char4.SetActive(false);
        // switch(selectedChar) {
        //   case 0:
        //     char1.SetActive(true);
        //     playerMesh = char1;
        //     break;
        //   case 1:
        //     char2.SetActive(true);
        //     playerMesh = char2;
        //     break;
        //   case 2:
        //     char3.SetActive(true);
        //     playerMesh = char3;
        //     break;
        //   case 3:
        //     char4.SetActive(true);
        //     playerMesh = char4;
        //     break;
        // }
    }

    void DisplayGameOverPanel() {
        gameStarted = false;
        gameOverPanel.SetActive(true);
        if (score.Equals(PlayerPrefs.GetInt("highestScore", 0)))
        {
            finalScore.text = "Your Score: " + score.ToString() + "\nNEW TOP!!";
        }
        else if(score> PlayerPrefs.GetInt("highestScore", 0)/2)
        {
            finalScore.text = "Your Score: " + score.ToString() + "\nGreat Score!";
        }
        else
        {
            finalScore.text = "Your Score: " + score.ToString() + "\nSweet!";
        }
    }

    void HideGameOverPanel() {
        gameOverPanel.SetActive(false);
    }

    void PlayAgain() {
        Debug.Log("Play again button pressed");
        PlayerPrefs.SetInt("reloaded", 1);
        // Reset level and start over
        SceneManager.LoadScene(REPLAY_SCENE);
    }
}
