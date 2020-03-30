using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterMenuButton : MonoBehaviour
      , IPointerEnterHandler
      , IPointerExitHandler
{

    public Text CharacterText;
    public int unlocked;
    public Image CharImage;
    public Image CharImageOutline;
    public GameObject Cost;
    public Text CostText;
    public Text CostTextOutline;
    public GameObject BeetImage;

    public void OnPointerEnter(PointerEventData eventData) {
      if (unlocked == 0) {
        Cost.SetActive(true);
        BeetImage.SetActive(true);
      }
    }

    public void OnPointerExit(PointerEventData eventData) {
      if (unlocked == 0) {
        Cost.SetActive(false);
        BeetImage.SetActive(false);
      }
    }
}
