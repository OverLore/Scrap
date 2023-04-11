using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Holdable
{
    [Header("References")]
    [SerializeField] private Animator animator;

    public bool IsUsing { get { return ManagersManager.instance.inputManager.Inputs.PlayerGround.PrimaryAction.ReadValue<float>() >= .3f; } }

    void Update()
    {
        if (owner == null || !owner.CanMove)
            return;
        
        animator.SetBool("Fire", IsUsing);
    }

    public void Gather()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f))
        {
            if (hit.collider.TryGetComponent(out Gatherable gatherable))
            {
                if (gatherable.Gather(InventoryController.Instance.hotbar.EquippedItem))
                {
                    GameObject go = Instantiate(gatherable.breakFX);
                    
                    go.transform.position = hit.point;
                    go.transform.LookAt(transform.position);

                    Destroy(go, 3f);
                }
            }
        }
    }
}
