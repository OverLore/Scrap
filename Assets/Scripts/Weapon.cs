using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] FPSController owner;

    private void Update()
    {
        animator.SetBool("IsSprinting", owner.IsTryingToSprint && !owner.IsCrouching);
        animator.SetBool("IsAiming", owner.IsAiming);
    }
}
