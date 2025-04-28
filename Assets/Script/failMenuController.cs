using UnityEngine;
using TMPro;

public class failMenuController : MonoBehaviour
{
    public TMP_Text menuText;

    public void changeText(string text) {
        menuText.text = text;
    }
}
