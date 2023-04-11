using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] CanvasGroup panel;
    [SerializeField] protected Canvas parentCanvas;

    public bool isShown;

    public void Show()
    {
        isShown = true;

        panel.alpha = 1;
        panel.blocksRaycasts = true;
        panel.interactable = true;
    }

    public void Hide()
    {
        isShown = false;

        panel.alpha = 0;
        panel.blocksRaycasts = false;
        panel.interactable = false;
    }
    
    public bool Toggle()
    {
        if (isShown)
            Hide();
        else
            Show();

        return isShown;
    }
}