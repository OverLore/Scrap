using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CursorSlot : Viewable
{
    public bool IsEmpty => item == null || item.currentAmount < 1;
    public bool IsFull => item != null && item.currentAmount >= item.maxStackSize;

    [Header("Components")]
    [SerializeField] private Transform origin;
    [SerializeField] private Image amountImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text amountText;

    private Item item;

    public override void Awake()
    {
        base.Awake();

        SetItem(null);
    }

    private void Update()
    {
        if (!View.IsEnabled)
            return;

        UpdatePosition();
    }

    void UpdatePosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Mouse.current.position.ReadValue(), canvas.worldCamera, out Vector2 pos);
        origin.position = canvas.transform.TransformPoint(pos);
    }

    public void Refresh()
    {
        amountText.text = string.Empty;
        itemImage.sprite = null;
        itemImage.SetAlpha(0);
        amountImage.SetAlpha(0);

        if (item == null)
            return;

        if (item.currentAmount < 1)
        {
            item = null;

            return;
        }

        amountText.text = item.currentAmount.ToString();
        itemImage.sprite = item.icon;

        itemImage.SetAlpha(1);
        amountImage.SetAlpha(1);
        
        UpdatePosition();
    }

    public void SetItem(Item _item)
    {
        if (_item)
            item = _item.Clone();
        else
            item = null;

        Refresh();
    }

    public Item GetItem()
    {
        return item;
    }

    public override void Hide()
    {
        base.Hide();

        //TODO: Drop item
        SetItem(null);
    }
}
