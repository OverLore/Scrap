using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableItem : MonoBehaviour
{
    protected FPSController owner;

    public void SetOwner(FPSController player)
    {
        owner = player;
    }
}
