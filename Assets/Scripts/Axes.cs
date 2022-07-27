using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Axes : HoldableItem
{
    public override void Awake()
    {
        base.Awake();

        animator.SetTrigger("Equip");
    }

    public override void Update()
    {
        base.Update();

        animator.SetBool("Enabled", Mouse.current.leftButton.isPressed && !owner.IsViewEnable);
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
                decallable.SetDecal(hit, IHitDecallabe.HitType.Axe);
            }

            if (gatherable != null)
            {
                gatherable.Gather(owner);
            }
        }
    }
}
