using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionScript : MonoBehaviour
{
    public GameObject characterMenuPanel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // CHARACTER MENU LISTENERS
    // Load character menu
    public void CharacterMenuButtonPressed() {
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
    }

    // Load char2
    void CharacterSelectedChar2() {
      Debug.Log("Character 2 Selected");
      PlayerPrefs.SetInt("selectedCharacter", 1);
      // Return to main menu after selection
      characterMenuPanel.SetActive(false);
    }

    // Load char3
    void CharacterSelectedChar3() {
      Debug.Log("Character 3 Selected");
      PlayerPrefs.SetInt("selectedCharacter", 2);
      // Return to main menu after selection
      characterMenuPanel.SetActive(false);
    }

    // Load char4
    void CharacterSelectedChar4() {
      Debug.Log("Character 4 Selected");
      PlayerPrefs.SetInt("selectedCharacter", 3);
      // Return to main menu after selection
      characterMenuPanel.SetActive(false);
    }
}
