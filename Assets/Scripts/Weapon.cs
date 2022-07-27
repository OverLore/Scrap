using UnityEngine;

public class Weapon : HoldableItem
{
    [SerializeField] Animator animator;
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

    [SerializeField] private float xRotationForce;
    [SerializeField] private float yRotationForce;

    [SerializeField] float mouseSensibility;
    [SerializeField] Vector2 xRotationLimits;
    [SerializeField] Vector2 yRotationLimits;

    private Vector3 origin;
    private float xOffset;
    private float yOffset;

    private float xRotationOffset;
    private float yRotationOffset;

    private void Awake()
    {
        origin = pivot.transform.localPosition;
    }

    private void Update()
    {
        animator.SetBool("IsSprinting", owner.IsTryingToSprint && !owner.IsCrouching);
        animator.SetBool("IsAiming", owner.IsAiming);

        Vector3 destination = origin;

        xRotationOffset = InputManager.Instance.Input.PlayerGround.Movement.ReadValue<Vector2>().x + InputManager.Instance.Input.PlayerGround.Look.ReadValue<Vector2>().x * mouseSensibility * owner.Sensibility.x;
        yRotationOffset = InputManager.Instance.Input.PlayerGround.Movement.ReadValue<Vector2>().y + InputManager.Instance.Input.PlayerGround.Look.ReadValue<Vector2>().y * mouseSensibility * owner.Sensibility.y;

        xRotationOffset = Mathf.Clamp(xRotationOffset, xRotationLimits.x, xRotationLimits.y);
        yRotationOffset = Mathf.Clamp(yRotationOffset, yRotationLimits.x, yRotationLimits.y);

        if (owner.IsSprinting && owner.IsMoving)
        {
            xOffset = Mathf.Cos(Time.time * sprintSpeedX) * sprintForce;
            yOffset = Mathf.Cos(Time.time * sprintSpeedY) * sprintForce;

            destination = new Vector3(origin.x + xOffset, origin.y + yOffset, origin.z);
        }
        else if (owner.IsCrouching && owner.IsMoving)
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

        pivot.transform.localPosition = Vector3.Lerp(pivot.transform.localPosition, destination, Time.deltaTime * (owner.IsAiming ? 10f : 5f));
        pivot.transform.localRotation = Quaternion.Lerp(pivot.transform.localRotation, Quaternion.Euler(yRotationOffset * yRotationForce, 0, -xRotationOffset * xRotationForce), Time.deltaTime * (owner.IsAiming ? 10f : 5f));
    }
}
