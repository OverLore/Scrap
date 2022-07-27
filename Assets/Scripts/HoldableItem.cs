using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableItem : MonoBehaviour
{
    protected FPSController owner;
    protected Transform root;

    [Header("Animator")]
    [SerializeField] protected Animator animator;

    [Space(10), Header("Sway")]
    [Header("Sprint")]
    [SerializeField] protected float sprintSpeedX;
    [SerializeField] protected float sprintSpeedY;
    [SerializeField] protected float sprintForce;
    [Header("Walk")]
    [SerializeField] protected float walkSpeedX;
    [SerializeField] protected float walkSpeedY;
    [SerializeField] protected float walkForce;
    [Header("Crouch")]
    [SerializeField] protected float crouchSpeedX;
    [SerializeField] protected float crouchSpeedY;
    [SerializeField] protected float crouchForce;
    [Header("Rotation")]
    [SerializeField] protected float xRotationForce;
    [SerializeField] protected float yRotationForce;
    [SerializeField] protected Vector2 xRotationLimits;
    [SerializeField] protected Vector2 yRotationLimits;
    [Header("Inputs")]
    [SerializeField] protected float mouseSensibility;

    protected float xOffset;
    protected float yOffset;

    protected float xRotationOffset;
    protected float yRotationOffset;
    protected Vector3 origin;

    public virtual void Awake()
    {
        animator.enabled = false;
    }

    public void SetOwner(FPSController player)
    {
        owner = player;

        root = player.GetHandRoot();
        origin = root.localPosition;
    }

    private void OnDestroy()
    {
        root.localRotation = Quaternion.identity;
        root.localPosition = Vector3.zero;
    }

    public virtual void Update()
    {
        if (!animator.enabled)
            animator.enabled = true;

        UpdateAnimationStates();

        ApplyMovingOffsets(ComputeMovingOffsets());
    }

    public virtual void UpdateAnimationStates()
    {
        animator.SetBool("IsSprinting", owner.IsSprinting && owner.IsMoving);
    }

    public virtual Vector3 ComputeMovingOffsets()
    {
        xRotationOffset = InputManager.Instance.Input.PlayerGround.Movement.ReadValue<Vector2>().x + InputManager.Instance.Input.PlayerGround.Look.ReadValue<Vector2>().x * mouseSensibility * owner.Sensibility.x;
        yRotationOffset = InputManager.Instance.Input.PlayerGround.Movement.ReadValue<Vector2>().y + InputManager.Instance.Input.PlayerGround.Look.ReadValue<Vector2>().y * mouseSensibility * owner.Sensibility.y;

        xRotationOffset = Mathf.Clamp(xRotationOffset, xRotationLimits.x, xRotationLimits.y);
        yRotationOffset = Mathf.Clamp(yRotationOffset, yRotationLimits.x, yRotationLimits.y);

        if (owner.IsViewEnable)
        {
            xRotationOffset = 0;
            yRotationOffset = 0;

            return origin;
        }

        if (owner.IsSprinting && owner.IsMoving)
        {
            xOffset = Mathf.Cos(Time.time * sprintSpeedX) * sprintForce;
            yOffset = Mathf.Cos(Time.time * sprintSpeedY) * sprintForce;

            return new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }

        if (owner.IsCrouching && owner.IsMoving)
        {
            xOffset = Mathf.Cos(Time.time * crouchSpeedX) * crouchForce;
            yOffset = Mathf.Cos(Time.time * crouchSpeedY) * crouchForce;

            return new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }

        if (owner.IsWalking)
        {
            xOffset = Mathf.Cos(Time.time * walkSpeedX) * walkForce;
            yOffset = Mathf.Cos(Time.time * walkSpeedY) * walkForce;

            return new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }

        return origin;
    }
    
    public virtual void ApplyMovingOffsets(Vector3 destination)
    {
        root.localPosition = Vector3.Lerp(root.localPosition, destination, Time.deltaTime * 5f);
        root.localRotation = Quaternion.Lerp(root.localRotation, Quaternion.Euler(yRotationOffset * yRotationForce, 0, -xRotationOffset * xRotationForce), Time.deltaTime * 5f);
    }
}
