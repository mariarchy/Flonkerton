using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSelectionScript : MonoBehaviour
{
    public GameObject CharacterMenuPanel;
    public GameObject InvalidPurchasePanel;
    public GameObject ConfirmationPurchasePanel;
    public Text SchruteBucksText;

    [System.Serializable]
    public class Character {
      public string Name;
      public int Unlocked;
      public bool IsInteractable;
      public Sprite CharImage;
      public Sprite CharImageOutline;
      public GameObject CharObject;
      public string Cost;

      public Button.ButtonClickedEvent OnClickEvent;

    }

    public GameObject buttonPrefab;
    public List<Character> CharacterList;
    public Transform Spacer;
    // Set default to Michael
    public CharacterMenuButton activeChar;
    public GameObject mainPlayer;
    private ArrayList charNameArray = new ArrayList();
    private List<CharacterMenuButton> allCharButtons = new List<CharacterMenuButton>();

    // Start is called before the first frame update
    void Start() {
      SchruteBucksText.text = PlayerPrefs.GetInt("schruteBucks").ToString();
      FillList();
    }

    void Update() {

    }

    void FillList() {
      int i = 0;
      foreach(var character in CharacterList) {
        GameObject newButton = Instantiate(buttonPrefab) as GameObject;
        newButton.transform.SetParent(Spacer);

        // Get C# script on button
        CharacterMenuButton charButton = newButton.GetComponent<CharacterMenuButton>();
        charButton.CharacterText.text = character.Name;
        // Add character name to array for reference in button listeners
        charNameArray.Add(character.Name);

        // Check if character is unlocked
        if (PlayerPrefs.GetInt("char" + charButton.CharacterText.text) == 1) {
          character.Unlocked = 1;
          // Set white character outline if unlocked
          charButton.CharImageOutline.sprite = character.CharImageOutline;
        } else {
          // Set grey overlay over character if its locked
          charButton.CharImage.color = new Color32(60,60,60,100);
          charButton.CharacterText.color = new Color32(255,255,255,70);
          charButton.CharImageOutline.color = new Color32(255,255,255,0);
        }
        // // Store its index in the c
        // PlayerPrefs.SetInt("char_index" + charButton.CharacterText.text, i);
        // Set new button's character name to input character name, cost, and image
        charButton.unlocked = character.Unlocked;
        charButton.CharImage.sprite = character.CharImage;
        charButton.CostText.text = character.Cost;
        charButton.CostTextOutline.text = character.Cost;

        charButton.GetComponent<Button>().interactable = true;
        // Add button click listener
        // Some weird binding issue here, so initialize a new var as a workaround
        int j = i;
        Debug.Log(j);
        Debug.Log(charButton.CharacterText.text);
        charButton.GetComponent<Button>().onClick.AddListener(() => ActivateCharacter(j));
        allCharButtons.Add(charButton);
        i++;
      }
      // Save characters
      SaveAll();
    }

    public void ActivateCharacter(int index) {
      // Player is unlocked
      activeChar = allCharButtons[index];
      Debug.Log(index);
      if (PlayerPrefs.GetInt("char" + activeChar.CharacterText.text) == 1) {
        //TODO: Activate character
        Debug.Log("Selecting: " + activeChar.CharacterText.text);
        PlayerPrefs.SetInt("selectedChar", index);
        CharacterMenuPanel.SetActive(false);
      } else {
        // If not enough schrute bucks, display message
        Debug.Log("Char " + activeChar.CharacterText.text + " costs " + activeChar.CostText.text);
        Debug.Log("You have " + PlayerPrefs.GetInt("schruteBucks").ToString());
        if (Int32.Parse(activeChar.CostText.text) > PlayerPrefs.GetInt("schruteBucks")) {
          // display not enough schrute bucks panel
          InvalidPurchasePanel.SetActive(true);
          // Reset active char
          activeChar = null;
        } else {
          // display confirmation purchase panel
          ConfirmationPurchasePanel.SetActive(true);
        }
      }
    }

    void PurchaseCharacter() {
      // Only make a purchase if a character has been selected
      Debug.Log("char to purchase: " + activeChar.CharacterText.text);
      if (activeChar != null) {
        int schruteBucks = PlayerPrefs.GetInt("schruteBucks");
        int cost = Int32.Parse(activeChar.CostText.text);
        PlayerPrefs.SetInt("schruteBucks", schruteBucks - cost);
        activeChar.unlocked = 1;
        SchruteBucksText.text = PlayerPrefs.GetInt("schruteBucks").ToString();

        ConfirmationPurchasePanel.SetActive(false);
        // Save unlocked character
        PlayerPrefs.SetInt("char" + activeChar.CharacterText.text, 1);
        // Remove grey overlay
        activeChar.CharImage.color = new Color32(255,255,255,255);
        activeChar.CharacterText.color = new Color32(255,255,255,255);
        activeChar.CharImageOutline.color = new Color32(255,255,255,255);
        // Reset active char
        activeChar = null;
      }
    }

    // Save lock/unlock status of each character
    void SaveAll() {
      foreach (CharacterMenuButton charButton in allCharButtons) {
        PlayerPrefs.SetInt("char" + charButton.CharacterText.text, charButton.unlocked);
      }
    }

    void DeleteAll() {
      PlayerPrefs.DeleteAll();
    }

    // CHARACTER MENU LISTENERS
    // Load character menu
    public void CharacterMenuButtonPressed() {
        Debug.Log("Character Menu Button Pressed");
        // Hide start menu panel & display menu panel
        CharacterMenuPanel.SetActive(true);
    }

    void HideInvalidPurchasePanel() {
      InvalidPurchasePanel.SetActive(false);
    }

    void HideConfirmationPanel() {
      ConfirmationPurchasePanel.SetActive(false);
    }
    // Load char1
    void CharacterSelectedChar1() {
      Debug.Log(charNameArray[0]);
      PlayerPrefs.SetInt("selectedCharacter", 0);
      // Return to main menu after selection
      CharacterMenuPanel.SetActive(false);
    }

    // Load char2
    void CharacterSelectedChar2() {
      Debug.Log("Character 2 Selected");
      PlayerPrefs.SetInt("selectedCharacter", 1);
      // Return to main menu after selection
      CharacterMenuPanel.SetActive(false);
    }

    // Load char3
    void CharacterSelectedChar3() {
      Debug.Log("Character 3 Selected");
      PlayerPrefs.SetInt("selectedCharacter", 2);
      // Return to main menu after selection
      CharacterMenuPanel.SetActive(false);
    }

    // Load char4
    void CharacterSelectedChar4() {
      Debug.Log("Character 4 Selected");
      PlayerPrefs.SetInt("selectedCharacter", 3);
      // Return to main menu after selection
      CharacterMenuPanel.SetActive(false);
    }

    // Load char5
    void CharacterSelectedChar5() {
      Debug.Log("Character 5 Selected");
      PlayerPrefs.SetInt("selectedCharacter", 4);
      // Return to main menu after selection
      CharacterMenuPanel.SetActive(false);
    }
}
