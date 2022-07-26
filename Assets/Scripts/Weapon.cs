using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] FPSController owner;
    [SerializeField] private GameObject pivot;
    [SerializeField] private float sprintSpeedX;
    [SerializeField] private float sprintSpeedY;
    [SerializeField] private float sprintForce;
    [SerializeField] private float walkSpeedX;
    [SerializeField] private float walkSpeedY;
    [SerializeField] private float walkForce;
    [SerializeField] private float crouchSpeedX;
    [SerializeField] private float crouchSpeedY;
    [SerializeField] private float crouchForce;

    private Vector3 origin;
    private float xOffset;
    private float yOffset;

    private void Awake()
    {
        origin = pivot.transform.localPosition;
    }

    private void Update()
    {
        animator.SetBool("IsSprinting", owner.IsTryingToSprint && !owner.IsCrouching);
        animator.SetBool("IsAiming", owner.IsAiming);

        Vector3 destination = origin;

        if (owner.IsSprinting)
        {
            xOffset = Mathf.Cos(Time.time * sprintSpeedX) * sprintForce;
            yOffset = Mathf.Cos(Time.time * sprintSpeedY) * sprintForce;

            destination = new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }
        else if (owner.IsCrouching)
        {
            xOffset = Mathf.Cos(Time.time * crouchSpeedX) * crouchForce;
            yOffset = Mathf.Cos(Time.time * crouchSpeedY) * crouchForce;

            destination = new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }
        else if (owner.IsWalking)
        {
            xOffset = Mathf.Cos(Time.time * walkSpeedX) * walkForce;
            yOffset = Mathf.Cos(Time.time * walkSpeedY) * walkForce;

            destination = new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }

        pivot.transform.localPosition = Vector3.Lerp(pivot.transform.localPosition, destination, Time.deltaTime * 5f);
    }
}
