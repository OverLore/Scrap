using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelNotification : InventoryPanel
{
    [Header("References")]
    [SerializeField] private NotificationController notificationController;
    [SerializeField] private Transform grid;
}
