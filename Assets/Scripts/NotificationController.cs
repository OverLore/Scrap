using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    private List<Notification> notifications = new List<Notification>();

    public static NotificationController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    [System.Serializable]
    public struct NotificationAvailable
    {
        public GameObject notifObj;
        public Notification.Type type;
        public string id;
    }

    [Header("References")]
    [SerializeField] private List<NotificationAvailable> availableNotification = new List<NotificationAvailable>();
    [SerializeField] float ySpacing = 55;
    [SerializeField] Transform grid;

    void Update()
    {
        for (int i = 0; i < notifications.Count; i++)
        {
            if (notifications[i] == null)
            {
                notifications.RemoveAt(i);
                i--;
                
                continue;
            }

            notifications[i].transform.localPosition = Vector3.Lerp(notifications[i].transform.localPosition, new Vector3(0, -i * ySpacing, 0), Time.deltaTime * 10f);
        }
    }

    public Notification AddNotification(string _type, string _body, string _value, float _timer, bool _constant, Action _action)
    {
        NotificationAvailable notifAvailable = availableNotification.Find(x => x.id == _type);

        if (notifAvailable.notifObj == null)
            return null;

        Notification notif = Instantiate(notifAvailable.notifObj, grid).GetComponent<Notification>();
        notif.transform.localPosition = new Vector3(500, 0, 0);

        notif.Setup(_body, _value, _timer).AddOnEachSecond(_action);

        notifications.Insert(_constant ? Mathf.Clamp(notifications.Count - 1, 0, Int32.MaxValue) : 0, notif);

        return notif;
    }
}
