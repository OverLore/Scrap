using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notification : MonoBehaviour
{
    public bool IsAlive => HasTimer ? timer > 0 : IsPersistent || lifeTime > 0;

    public bool IsPersistent = false;
    public bool HasTimer = false;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Animator animator;

    private string title;
    private string value;
    private float timer;
    private string format;
    private float lifeTime = 5f;

    void Refresh()
    {
        titleText.text = title;
        valueText.text = HasTimer ? format.Replace("%time%", ((int)timer).ToString()) : value;
    }

    private void Update()
    {
        if (HasTimer)
            timer -= Time.deltaTime;
        else if (!IsPersistent)
            lifeTime -= Time.deltaTime;

        Refresh();
    }

    public void Kill()
    {
        animator.SetTrigger("Depop");
        Destroy(gameObject, 1f);
    }

    public void SetupPersistent(string _title, string _value)
    {
        IsPersistent = true;

        title = _title;
        value = _value;

        Refresh();
    }

    public void SetupTimed(string _title, string _value, float time, string _format)
    {
        HasTimer = true;

        title = _title;
        value = _value;
        timer = time;
        format = _format;

        Refresh();
    }

    public void Setup(string _title, string _value)
    {
        title = _title;
        value = _value;

        Refresh();
    }
}
