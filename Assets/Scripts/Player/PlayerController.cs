using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("MoveSpeed")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4.5f;
    [SerializeField] private float crouchSpeed = 1.2f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float backwardSpeedMultiplier = 0.7f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0.9f, 0);

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 30f;
    [SerializeField] private float staminaRegenRate = 20f;
    [SerializeField] private float staminaRegenDelay = 1.5f;
    [SerializeField] private float exhaustedSpeedMultiplier = 0.6f;
    [SerializeField] private float exhaustedThreshold = 20f;

    [Header("QuickTurn")]
    [SerializeField] private float quickTurnTime = 0.3f;

    [Header("Physics")]
    [SerializeField] private float groundDrag = 8f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Camera mainCamera;

    private Vector2 inputDirection;
    private Vector3 moveDirection;
    private Vector3 cameraRelativeMove;
    private float currentSpeed;
    private bool isGrounded;

    private bool isRunning;
    private bool isCrouching;
    private bool isQuickTurning;
    private bool isExhausted;
    private bool wantsToCrouch;

    private float currentStamina;
    private float staminaRegenTimer;

    private float quickTurnTimer;
    private Quaternion quickTurnStartRot;
    private Quaternion quickTurnTargetRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main;

        capsuleCollider.height = standingHeight;
        capsuleCollider.center = standingCenter;
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleInput();
        HandleGroundCheck();
        HandleCrouch();
        HandleStamina();
        HandleQuickTurnInput();

        UpdateMovementDirection();
    }

    void FixedUpdate()
    {
        if (isQuickTurning)
        {
            HandleQuickTurn();
        }
        else
        {
            HandleMovement();
            //HandleRotation();
        }

        HandleVerticalSnap();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        inputDirection = new Vector2(horizontal, vertical);

        if (inputDirection.magnitude > 1f)
        {
            inputDirection.Normalize();
        }

        bool runButton = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire3");
        isRunning = runButton && !isCrouching && !isExhausted && inputDirection.magnitude > 0.1f;

        wantsToCrouch = Input.GetKey(KeyCode.C);

        if (Input.GetKeyDown(KeyCode.Space) && !isQuickTurning && isGrounded && !isCrouching)
        {
            StartQuickTurn();
        }
    }

    private void UpdateMovementDirection()
    {
        if (mainCamera == null) return;

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        cameraRelativeMove = (cameraForward * inputDirection.y + cameraRight * inputDirection.x).normalized;

        if (inputDirection.magnitude > 0.1f)
        {
            moveDirection = cameraRelativeMove;
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }

    private void HandleMovement()
    {
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isRunning)
        {
            currentSpeed = runSpeed;

            if (inputDirection.magnitude > 0.1f)
            {
                currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
                currentStamina = Mathf.Max(0, currentStamina);
                staminaRegenTimer = 0f;

                if (currentStamina <= 0)
                {
                    isExhausted = true;
                    isRunning = false;
                }
            }
        }
        else if (isExhausted)
        {
            currentSpeed = walkSpeed * exhaustedSpeedMultiplier;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        float speedMultiplier = 1f;
        if (inputDirection.y < -0.1f)
        {
            speedMultiplier = backwardSpeedMultiplier;
        }

        Vector3 currentVelocity;
        Vector3 newVelocity;

        if (moveDirection.magnitude < 0.1f)
        {
            currentVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            newVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
            return;
        }

        Vector3 targetVelocity = moveDirection * currentSpeed * speedMultiplier;

        currentVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        newVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
    }

    //private void HandleRotation()
    //{
    //    if (moveDirection.magnitude < 0.1f) return;

    //    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    //    Quaternion newRotation = Quaternion.Slerp(
    //        transform.rotation,
    //        targetRotation,
    //        rotationSpeed * Time.fixedDeltaTime
    //    );

    //    rb.MoveRotation(newRotation);
    //}

    private void StartQuickTurn()
    {
        isQuickTurning = true;
        quickTurnTimer = 0f;
        quickTurnStartRot = transform.rotation;
        quickTurnTargetRot = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);

        rb.linearVelocity = Vector3.zero;
    }

    private void HandleQuickTurn()
    {
        quickTurnTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(quickTurnTimer / quickTurnTime);

        float easedT = 1f - Mathf.Pow(1f - t, 3);

        transform.rotation = Quaternion.Slerp(quickTurnStartRot, quickTurnTargetRot, easedT);

        if (t > 0.7f)
        {
            Vector3 forwardMove = transform.forward * walkSpeed * 0.3f * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forwardMove);
        }

        if (t >= 1f)
        {
            isQuickTurning = false;
        }
    }

    private void HandleQuickTurnInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleQuickTurn();
        }
    }

    private void HandleCrouch()
    {
        if (wantsToCrouch && !isCrouching && !isRunning)
        {
            if (CanCrouch())
            {
                isCrouching = true;
            }
        }
        else if (!wantsToCrouch && isCrouching)
        {
            if (CanStandUp())
            {
                isCrouching = false;
            }
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        Vector3 targetCenter = isCrouching ? crouchCenter : standingCenter;

        capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        capsuleCollider.center = Vector3.Lerp(capsuleCollider.center, targetCenter, crouchTransitionSpeed * Time.deltaTime);
    }

    private bool CanCrouch()
    {
        return true;
    }

    private bool CanStandUp()
    {
        Vector3 rayStart = transform.position + Vector3.up * (crouchHeight + 0.1f);
        float checkDistance = standingHeight - crouchHeight - 0.2f;

        return !Physics.Raycast(rayStart, Vector3.up, checkDistance, groundMask);
    }

    private void HandleStamina()
    {
        if (isRunning)
        {
            staminaRegenTimer = 0f;
        }
        else
        {
            staminaRegenTimer += Time.deltaTime;

            if (staminaRegenTimer >= staminaRegenDelay && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);

                if (currentStamina > exhaustedThreshold)
                {
                    isExhausted = false;
                }
            }
        }

        if (currentStamina <= 0)
        {
            isExhausted = true;
        }
    }

    private void HandleGroundCheck()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance + 0.1f, groundMask);

        rb.linearDamping = isGrounded ? groundDrag : 0f;
    }

    private void HandleVerticalSnap()
    {
        if (isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }

    public float GetStaminaNormalized() => currentStamina / maxStamina;
    public bool IsRunning() => isRunning;
    public bool IsCrouching() => isCrouching;
    public bool IsExhausted() => isExhausted;
    public bool IsMoving() => inputDirection.magnitude > 0.1f;
    public Vector3 GetMoveDirection() => moveDirection;
    public Vector2 GetInputDirection() => inputDirection;
}