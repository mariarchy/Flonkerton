using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    public GameObject mainCharacter;
    public float speed = 3F;
    public float regularSpeed = 3F;
    public float maxSpeed = 7F;
    public const float CLOSE_DISTANCE = 15;

    private bool gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
	// If player reloaded the game, start ghost movement
        if (PlayerPrefs.GetInt("play") == 1) {
            Debug.Log("Move ghost");
	        gameStarted = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
	// Check status of game
	if (!gameStarted) {
        // Debug.Log("Ghost movement is OFF");
	    return;
    }

	// Adjust ghost speed depending on distance from the player
	float distance = Vector3.Distance(mainCharacter.transform.position, this.transform.position);
	if (distance < CLOSE_DISTANCE)
	{
	    speed = regularSpeed;
	}
	else
	{
	    speed = maxSpeed;
	}
	// Move ghost towards character
        this.transform.position = Vector3.MoveTowards(this.transform.position, mainCharacter.transform.position, speed * Time.fixedDeltaTime);
	// Rotate ghost so it's "looking at" the main character
	this.transform.LookAt(mainCharacter.transform.position);
    }

    public void GameStarted()
    {
      Debug.Log("Game Started");
	    gameStarted = true;
    }

}
