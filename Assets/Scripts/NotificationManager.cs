using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public enum NotificationStyle
    {
        InventoryAdd,
        CraftInProgress,
        Bleeding,
        Hunger,
        Thirst,
        Info
    }
    public enum NotificationType
    {
        Normal,
        Persistent,
        Timed
    }

    [System.Serializable]
    public class NotifStyle
    {
        public NotificationStyle type;
        public GameObject prefab;
    }

    [SerializeField] private List<NotifStyle> styles = new List<NotifStyle>();
    [SerializeField] private RectTransform bottom;
    [SerializeField] private RectTransform top;

    private List<Notification> notifications = new List<Notification>();

    void Reorder()
    {
        float height = styles[0].prefab.GetComponent<Image>().rectTransform.rect.height;
        float space = 2f;

        for (int i = notifications.Count - 1; i >= 0; i--)
        {
            Vector2 notifPosition = notifications[i].GetComponent<Image>().rectTransform.anchoredPosition;

            float yDest = i * (height + space) + bottom.anchoredPosition.y;
            float newY = Mathf.Lerp(notifPosition.y, yDest,
                Time.deltaTime * 10f);

            notifications[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(notifPosition.x, newY);
        }
    }

    private void Update()
    {
        if (notifications == null || notifications.Count == 0)
            return;

        foreach (Notification notif in notifications)
        {
            if (notif.IsAlive)
                continue;

            notifications.Remove(notif);
            notif.Kill();
        }

        Reorder();
    }

    void RemoveOlder()
    {
        for (int i = notifications.Count - 1; i >= 0; i--)
        {
            if (!notifications[i].IsPersistent || !notifications[i].HasTimer)
                continue;

            notifications[i].Kill();
            notifications.Remove(notifications[i]);
        }
    }

    GameObject GetNotificationBase(NotificationStyle _style)
    {
        foreach (NotifStyle style in styles)
        {
            if (style.type == _style)
                return style.prefab;
        }

        return null;
    }

    public Notification AddNotification(NotificationType type, NotificationStyle style, string title, string value, float timer = 0, string format = "")
    {
        GameObject notifObj = Instantiate(GetNotificationBase(style), transform);
        notifObj.GetComponent<Image>().rectTransform.anchoredPosition = top.anchoredPosition;
        Notification notif = notifObj.GetComponent<Notification>();

        switch (type)
        {
            case NotificationType.Normal:
                notif.Setup(title, value);
                break;
            case NotificationType.Persistent:
                notif.SetupPersistent(title, value);
                break;
            case NotificationType.Timed:
                notif.SetupTimed(title, value, timer, format);
                break;
        }

        InsertNotification(notif, type);

        return notif;
    }

    public void ForceKillNotification(Notification notif)
    {
        notifications.Remove(notif);
    }

    void InsertNotification(Notification notif, NotificationType type)
    {
        if (notifications.Count > 10)
        {
            RemoveOlder();
        }

        switch (type)
        {
            case NotificationType.Normal:
            case NotificationType.Timed:
                notifications.Add(notif);
                break;
            case NotificationType.Persistent:
                notifications.Insert(0, notif);
                break;
        }
    }
}
