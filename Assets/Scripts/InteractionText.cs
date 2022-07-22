using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionText : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    private void Update()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }
    
    public void ShowText(string _text)
    {
        text.text = _text;

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }
}
