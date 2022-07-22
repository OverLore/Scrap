using System.Collections.Generic;

public class View
{
    public bool IsEmpty => m_viewables.Count == 1;

    public View(FPSController _owner)
    {
        owner = _owner;
        playerInventory = _owner.GetInventory();

        playerInventory.SetView(this);
    }

    public bool IsEnabled;

    private FPSController owner;
    private Inventory playerInventory;
    private List<Viewable> m_viewables = new List<Viewable>();

    public void AddViewable(Viewable _viewable)
    {
        if (m_viewables.Contains(_viewable))
            return;

        m_viewables.Add(_viewable);

        EnableView();
    }

    public CursorSlot GetCursor()
    {
        return playerInventory.Owner.GetCursor();
    }

    public void RemoveViewable(Viewable _viewable)
    {
        m_viewables.Remove(_viewable);

        if (m_viewables.Count == 0)
            DisableView();
    }

    public void RefreshView()
    {
        foreach (Viewable viewable in m_viewables)
        {
            if (IsEnabled)
                viewable.Show();
            else
                viewable.Hide();
        }

        if (IsEnabled)
        {
            playerInventory.Show();
            owner.GetCursor().Show();
        }
        else
        {
            playerInventory.Hide();
            owner.GetCursor().Hide();
        }
    }

    public void EnableView()
    {
        IsEnabled = true;

        CursorUtilities.UnlockCursor();

        RefreshView();
    }

    public void DisableView()
    {
        IsEnabled = false;

        CursorUtilities.LockCursor();

        RefreshView();

        m_viewables.Clear();
    }

    public bool HasMultipleInventories()
    {
        foreach (Viewable viewable in m_viewables)
        {
            if (viewable.GetComponent<Inventory>() != null && viewable.GetComponent<Inventory>() != playerInventory)
            {
                return false;
            }
        }

        return true;
    }

    public Inventory GetFirstOtherInventory()
    {
        foreach (Viewable viewable in m_viewables)
        {
            if (viewable.GetComponent<Inventory>() != null && viewable.GetComponent<Inventory>() != playerInventory)
            {
                return viewable.GetComponent<Inventory>();
            }
        }

        return null;
    }
}
