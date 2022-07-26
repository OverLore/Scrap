using System.Collections;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public bool CanMove => true;
    public bool IsAiming => InputManager.Instance.Input.PlayerGround.Aim.ReadValue<float>() > .3f;
    public bool IsTryingToSprint => InputManager.Instance.Input.PlayerGround.Sprint.ReadValue<float>() > .3f;
    public bool IsSprinting => IsTryingToSprint && !IsCrouching && !IsAiming;
    public bool IsCrouching => InputManager.Instance.Input.PlayerGround.Crouch.ReadValue<float>() > .3f;

    [Header("Movement settings")]
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpForce;

    [Space(10), Header("Look settings")]
    [SerializeField] private float lookSpeedX = 2f;
    [SerializeField] private float lookSpeedY = 2f;
    [SerializeField] private float lookLimitUp;
    [SerializeField] private float lookLimitDown;
    [SerializeField] LayerMask interactionLayers;
    [SerializeField] public LayerMask gatherableLayers;
    [SerializeField] private InteractionText interactionText;

    [Space(10), Header("Crouch")]
    [SerializeField] private float crouchHeight = .5f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float timeToCrouch = .25f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, .5f, 0);
    [SerializeField] private Vector3 standCenter = new Vector3(0, 0, 0);
    private float crouchAdvancement = 0;

    [Space(10), Header("Inventory")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private CursorSlot cursor;
    [SerializeField] private Transform handRoot;
    [SerializeField] private NotificationManager notificationManager;

    private Camera m_playerCam;
    private CharacterController m_controller;

    private Vector2 m_currentInput;
    private Vector3 m_direction;

    private float m_yRot;

    private float lastYVel;

    private View view;

    private void Awake()
    {
        InitComponents();

        CursorUtilities.LockCursor();

        InitView();
    }

    private void Update()
    {
        if (!CanMove)
            return;

        HandleMoveInput();
        HandleMouseLook();

        HandleInteractableRayCast();

        HandleCrouch();
        HandleJump();

        HandleInventory();

        ApplyFinalMovement();

        lastYVel = m_direction.y;
    }

    private void InitView()
    {
        view = new View(this);

        inventory.Owner = this;
        hotbar.View = view;
        hotbar.Owner = this;
        cursor.View = view;
    }

    private void InitComponents()
    {
        m_playerCam = GetComponentInChildren<Camera>();
        m_controller = GetComponent<CharacterController>();
    }

    private void HandleMoveInput()
    {
        if (view.IsEnabled)
        {
            // stop movements except gravity
            float mDirectionY = m_direction.y;
            m_direction = Vector3.zero;
            m_direction.y = mDirectionY;
            return;
        }

        m_currentInput = InputManager.Instance.Input.PlayerGround.Movement.ReadValue<Vector2>() * (IsSprinting ? ComputeSprintForce() : IsCrouching ? crouchSpeed : walkSpeed);

        float directionY = m_direction.y;
        m_direction = transform.forward * m_currentInput.y + transform.right * m_currentInput.x;
        m_direction.y = directionY;
    }

    //This function compute the force of the sprint according to the player move direction
    //Player can't run backward and run at its max speed when going forward
    private float ComputeSprintForce()
    {
        Vector3 normInput = m_currentInput.normalized;
        normInput = new Vector3(normInput.x, 0, normInput.y);
        normInput = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * normInput;

        float dotProduct = Vector3.Dot(normInput, transform.forward);

        float speedDif = sprintSpeed - walkSpeed;
        float halfSpeedDif = speedDif / 2.0f;

        return speedDif + halfSpeedDif + halfSpeedDif * dotProduct;
    }

    private void HandleMouseLook()
    {
        if (view.IsEnabled)
            return;

        Vector2 mouseInput = InputManager.Instance.Input.PlayerGround.Look.ReadValue<Vector2>();

        if (mouseInput == Vector2.zero)
        {
            return;
        }

        m_yRot -= mouseInput.y * lookSpeedY;
        m_yRot = Mathf.Clamp(m_yRot, lookLimitDown, lookLimitUp);

        m_playerCam.transform.localRotation = Quaternion.Euler(m_yRot, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseInput.x * lookSpeedX, 0);
    }

    private void ApplyFinalMovement()
    {
        if (!m_controller.isGrounded)
        {
            m_direction.y -= gravity * Time.deltaTime;
        }

        m_controller.Move(m_direction * Time.deltaTime);

        if (lastYVel == m_direction.y)
        {
            m_direction.y = -1f;
        }
    }

    private void HandleJump()
    {
        if (view.IsEnabled)
            return;

        if (InputManager.Instance.Input.PlayerGround.Jump.triggered && m_controller.isGrounded)
            m_direction.y = jumpForce;
    }

    void HandleCrouch()
    {
        if (view.IsEnabled)
            return;

        if (!IsCrouching && Physics.Raycast(m_playerCam.transform.position, Vector3.up, 1f))
        {
            return;
        }

        crouchAdvancement += (IsCrouching ? Time.deltaTime : -Time.deltaTime) / timeToCrouch;
        crouchAdvancement = Mathf.Clamp01(crouchAdvancement);

        m_controller.height = Mathf.Lerp(standHeight, crouchHeight, crouchAdvancement);
        m_controller.center = Vector3.Lerp(standCenter, crouchCenter, crouchAdvancement);
    }

    void HandleInteractableRayCast()
    {
        if (view.IsEnabled)
            return;

        if (Physics.Raycast(m_playerCam.transform.position, m_playerCam.transform.forward, out RaycastHit hit, 2, interactionLayers))
        {
            IInteractable interact = hit.transform.gameObject.GetComponent<IInteractable>();

            interact?.SetInteractionText(interactionText);

            if (InputManager.Instance.Input.PlayerGround.Interaction.triggered)
            {
                interact?.PerformInteraction(this);
            }
        }
    }

    private void HandleInventory()
    {
        if (!InputManager.Instance.Input.PlayerGround.Inventory.triggered)
            return;

        if (view.IsEnabled)
            view.DisableView();
        else
            view.AddViewable(inventory);
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public CursorSlot GetCursor()
    {
        return cursor;
    }

    public Hotbar GetHotbar()
    {
        return hotbar;
    }

    public InteractionText GetInteractionText()
    {
        return interactionText;
    }

    public Transform GetHandRoot()
    {
        return handRoot;
    }

    public Camera GetCamera()
    {
        return m_playerCam;
    }

    public NotificationManager GetNotificationManager()
    {
        return notificationManager;
    }
}
