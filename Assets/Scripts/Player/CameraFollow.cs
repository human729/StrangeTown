// CameraFollow.cs - исправленная версия
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.7f, -2.5f);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Follow Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;
    [SerializeField] private bool invertY = false;

    [Header("Collisions")]
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float collisionOffset = 0.2f;
    [SerializeField] private float cameraSmoothReturn = 8f;

    [Header("Aiming")]
    [SerializeField] private Vector3 aimOffset = new Vector3(0, 1.5f, -1f);
    [SerializeField] private float aimFOV = 40f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private float aimTransitionSpeed = 10f;
    [SerializeField] private float aimSensitivityMultiplier = 0.5f;

    [Header("Shoulder Switching")]
    [SerializeField] private bool enableShoulderSwitch = true;
    [SerializeField] private KeyCode switchShoulderKey = KeyCode.E;
    [SerializeField] private float shoulderOffset = 0.5f;
    private int shoulderSide = 1; // 1 = right, -1 = left

    [Header("Camera Effects")]
    [SerializeField] private bool enableCameraInertia = true;
    [SerializeField] private float inertiaAmount = 0.1f;
    [SerializeField] private float positionLagSpeed = 15f;

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private Vector3 smoothedPosition;
    private Vector3 velocityPosition;
    private Camera cam;
    private bool isAiming = false;
    private float targetFOV;
    private Vector3 desiredPosition;
    private Vector3 lastDesiredPosition;
    private float currentCollisionDistance = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentRotationY = target.eulerAngles.y;
        targetFOV = normalFOV;
        cam.fieldOfView = normalFOV;

        smoothedPosition = target.position + offset;
        lastDesiredPosition = smoothedPosition;
        currentCollisionDistance = offset.magnitude;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleInput();
        HandleCameraRotation();
        HandleShoulderSwitch();

        Vector3 targetOffset = CalculateTargetOffset();
        CalculateDesiredPosition(targetOffset);
        HandleCameraCollision();

        UpdateCameraPosition();
        UpdateFOV();
        UpdateCrosshair();

        // Всегда поворачиваем персонажа за камерой
        RotateCharacter();
    }

    private void HandleInput()
    {
        // Прицеливание
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
            targetFOV = aimFOV;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            targetFOV = normalFOV;
        }
    }

    private void HandleCameraRotation()
    {
        float sensitivity = isAiming ? mouseSensitivity * aimSensitivityMultiplier : mouseSensitivity;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * (invertY ? -1 : 1);

        currentRotationY += mouseX;
        currentRotationX -= mouseY;
        currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);
    }

    private void HandleShoulderSwitch()
    {
        if (enableShoulderSwitch && Input.GetKeyDown(switchShoulderKey))
        {
            shoulderSide *= -1;
        }
    }

    private Vector3 CalculateTargetOffset()
    {
        Vector3 baseOffset = isAiming ? aimOffset : offset;

        if (enableShoulderSwitch)
        {
            baseOffset.x = shoulderSide * Mathf.Abs(baseOffset.x);
        }

        return baseOffset;
    }

    private void CalculateDesiredPosition(Vector3 targetOffset)
    {
        // Всегда используем вращение камеры для расчета позиции
        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        desiredPosition = target.position + rotation * targetOffset;
    }

    private void HandleCameraCollision()
    {
        Vector3 direction = desiredPosition - target.position;
        float desiredDistance = direction.magnitude;

        RaycastHit hit;
        if (Physics.SphereCast(target.position, collisionRadius, direction.normalized,
            out hit, desiredDistance, collisionMask))
        {
            currentCollisionDistance = Mathf.Lerp(currentCollisionDistance,
                hit.distance - collisionOffset, cameraSmoothReturn * Time.deltaTime);
        }
        else
        {
            currentCollisionDistance = Mathf.Lerp(currentCollisionDistance,
                desiredDistance, cameraSmoothReturn * Time.deltaTime);
        }

        desiredPosition = target.position + direction.normalized * currentCollisionDistance;
    }

    private void UpdateCameraPosition()
    {
        if (enableCameraInertia && !isAiming)
        {
            Vector3 inertiaOffset = (desiredPosition - lastDesiredPosition) * inertiaAmount;
            desiredPosition += inertiaOffset;

            smoothedPosition = Vector3.SmoothDamp(smoothedPosition, desiredPosition,
                ref velocityPosition, 1f / positionLagSpeed);
        }
        else
        {
            smoothedPosition = Vector3.Lerp(smoothedPosition, desiredPosition,
                followSpeed * Time.deltaTime);
        }

        transform.position = smoothedPosition;
        lastDesiredPosition = desiredPosition;

        // Поворот камеры для взгляда на цель
        if (isAiming)
        {
            // При прицеливании смотрим немного выше для лучшего обзора
            Vector3 lookTarget = target.position + Vector3.up * 1.5f + target.forward * 2f;
            transform.LookAt(lookTarget);
        }
        else
        {
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
    }

    private void UpdateFOV()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV,
            aimTransitionSpeed * Time.deltaTime);
    }

    private void UpdateCrosshair()
    {
        if (crosshair != null)
        {
            crosshair.SetActive(isAiming);
        }
    }

    private void RotateCharacter()
    {
        // Поворачиваем персонажа вместе с камерой (только по горизонтали)
        Vector3 characterRotation = target.eulerAngles;
        characterRotation.y = currentRotationY;
        target.rotation = Quaternion.Slerp(target.rotation,
            Quaternion.Euler(characterRotation), rotationSpeed * Time.deltaTime);
    }

    // Публичные методы
    public bool IsAiming() => isAiming;
    public void SetTarget(Transform newTarget) => target = newTarget;

    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    public Vector3 GetCameraRight()
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right.normalized;
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.position, collisionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.position, desiredPosition);
    }
}