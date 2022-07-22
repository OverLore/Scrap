using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxes : HoldableItem
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator.SetTrigger("Equip");
    }

    private void Update()
    {
        animator.SetBool("Enabled", Mouse.current.leftButton.isPressed);
    }

    public void Hit()
    {
        Transform cam = owner.GetCamera().transform;

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 1.2f, owner.gatherableLayers))
        {
            Debug.Log(hit.transform.gameObject);

            Gatherable gatherable = hit.transform.gameObject.GetComponent<Gatherable>();
            IHitDecallabe decallable = hit.transform.gameObject.GetComponent<IHitDecallabe>();

            if (decallable != null)
            {
                decallable.SetDecal(hit, IHitDecallabe.HitType.Pickaxe);
            }

            if (gatherable != null)
            {
                gatherable.Gather(owner);
            }
        }
    }
}
