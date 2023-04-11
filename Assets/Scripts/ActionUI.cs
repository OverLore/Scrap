using UnityEngine;
using TMPro;

public class ActionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text actionText;
    [SerializeField] float speedFadeTxt;

    private float m_fadeProgress;
    private bool m_isActive;

    public static ActionUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Instance.m_fadeProgress = 0;
        m_isActive = false;
    }

    void Update()
    {
        m_fadeProgress += Time.deltaTime * speedFadeTxt * (m_isActive ? 1 : -1);
        m_fadeProgress = Mathf.Clamp01(m_fadeProgress);

        Color col = actionText.color;
        col.a = m_fadeProgress;
        actionText.color = col;

        m_isActive = false;
    }

    public void SetVisible()
    {
        m_isActive = true;
    }

    public void SetText(string _text)
    {
        Instance.actionText.text = _text;
    }
}
