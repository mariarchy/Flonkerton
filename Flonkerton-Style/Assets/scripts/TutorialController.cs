using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{

    // Sprites for tutorial splash screens
    public Sprite imgTutorial1;
    public Sprite imgTutorial2;
    public Sprite imgTutorial3;
    public Sprite imgTutorial4;

    public int tutorialPageNumber;
 
    // Start is called before the first frame update
    void Start()
    {
        tutorialPageNumber = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowNextTutorialPage() {
        // Switch to next splash screen when Next button is clicked
        switch (tutorialPageNumber) {
            case 1:
                GetComponent<Image>().sprite = imgTutorial2;
                tutorialPageNumber++;
                break;
            case 2:
                GetComponent<Image>().sprite = imgTutorial3;
                tutorialPageNumber++;
                break;
            case 3:
                GetComponent<Image>().sprite = imgTutorial4;
                tutorialPageNumber++;
                break;
            case 4:
                SceneManager.LoadScene("Flonkerton_Scene");
                break;
            default:
                Debug.Log("Tutorial Page Number Error");
                break;
        }
    }
}
