using UnityEngine;

public class Viewable : MonoBehaviour
{
    public bool IsEnabled => alwaysActive || (View != null && View.IsEnabled && canvas.enabled);

    protected Canvas canvas;
    public View View;
    public bool alwaysActive;

    public virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    public virtual void Show()
    {
        canvas.enabled = true;
    }

    public virtual void Hide()
    {
        canvas.enabled = false;
    }

    public void SetView(View _view)
    {
        View = _view;
    }
}
