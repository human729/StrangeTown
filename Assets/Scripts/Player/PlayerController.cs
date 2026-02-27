using UnityEngine;

public class PlayerControllerCinemachine : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4.5f;
    [SerializeField] private float crouchSpeed = 1.2f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float backwardSpeedMultiplier = 0.7f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private float crouchTransitionSpeed = 10f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 30f;
    [SerializeField] private float staminaRegenRate = 20f;
    [SerializeField] private float staminaRegenDelay = 1.5f;

    [Header("Physics")]
    [SerializeField] private float groundDrag = 8f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private CameraFollow cameraController;

    private Vector2 inputDirection;
    private Vector3 moveDirection;
    private float currentSpeed;
    private bool isGrounded;
    private bool isRunning;
    private bool isCrouching;
    private bool wantsToCrouch;

    private float currentStamina;
    private float staminaRegenTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        cameraController = Camera.main.GetComponent<CameraFollow>();

        capsuleCollider.height = standingHeight;
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleInput();
        HandleGroundCheck();
        HandleCrouch();
        HandleStamina();

        UpdateMovementDirection();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        HandleVerticalSnap();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        inputDirection = new Vector2(horizontal, vertical).normalized;

        bool runButton = Input.GetKey(KeyCode.LeftShift);
        isRunning = runButton && !isCrouching && inputDirection.magnitude > 0.1f && currentStamina > 0;

        wantsToCrouch = Input.GetKey(KeyCode.C);
    }

    private void UpdateMovementDirection()
    {
        if (cameraController == null) return;

        Vector3 cameraForward = cameraController.GetCameraForward();
        Vector3 cameraRight = cameraController.GetCameraRight();

        moveDirection = (cameraForward * inputDirection.y + cameraRight * inputDirection.x).normalized;
    }

    private void HandleMovement()
    {
        float targetSpeed = walkSpeed;

        if (isCrouching)
            targetSpeed = crouchSpeed;
        else if (isRunning)
            targetSpeed = runSpeed;

        float speedMultiplier = inputDirection.y < -0.1f ? backwardSpeedMultiplier : 1f;

        Vector3 currentVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 targetVelocity = moveDirection * targetSpeed * speedMultiplier;

        float accelerationRate = moveDirection.magnitude > 0.1f ? acceleration : deceleration;
        Vector3 newVelocity = Vector3.Lerp(currentVelocity, targetVelocity, accelerationRate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);

        // Расход Stamina
        if (isRunning && moveDirection.magnitude > 0.1f)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(0, currentStamina);
            staminaRegenTimer = 0f;
        }
    }

    private void HandleRotation()
    {
        if (moveDirection.magnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (wantsToCrouch && !isCrouching)
        {
            isCrouching = true;
        }
        else if (!wantsToCrouch && isCrouching && CanStandUp())
        {
            isCrouching = false;
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
    }

    private bool CanStandUp()
    {
        RaycastHit hit;
        return !Physics.SphereCast(transform.position + Vector3.up * crouchHeight,
            0.3f, Vector3.up, out hit, standingHeight - crouchHeight, groundMask);
    }

    private void HandleStamina()
    {
        if (!isRunning)
        {
            staminaRegenTimer += Time.deltaTime;

            if (staminaRegenTimer >= staminaRegenDelay && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down,
            groundCheckDistance + 0.1f, groundMask);

        rb.linearDamping = isGrounded ? groundDrag : 0f;
    }

    private void HandleVerticalSnap()
    {
        if (isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }

    public bool IsRunning() => isRunning;
    public bool IsCrouching() => isCrouching;
    public float GetStaminaNormalized() => currentStamina / maxStamina;
}