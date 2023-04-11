using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{
    public enum Type
    {
        Info,
        Warning,
        Error,
        Inventory,
        Crafting,
        Tip,
        DebugWarning,
        DebugError
    }

    [Header("References")]
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Type type;

    public Action onEachSecond;

    public float timer;

    void Awake()
    {
        StartCoroutine(UpdateSecondTimer());
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
            Destroy(gameObject);
    }

    public Notification Setup(string _body, string _value, float _timer)
    {
        bodyText.text = _body;
        valueText.text = _value;
        timer = _timer;

        return this;
    }
    
    public Notification AddOnEachSecond(Action _action)
    {
        onEachSecond += _action;

        return this;
    }

    public void SetValue(string _text)
    {
        valueText.text = _text;
    }

    IEnumerator UpdateSecondTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            onEachSecond?.Invoke();
        }
    }

}
