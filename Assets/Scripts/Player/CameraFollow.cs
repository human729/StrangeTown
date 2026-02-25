using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("MainSettings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.7f, -2.5f);
    [SerializeField] private float followSpeed = 10f;

    [Header("Follow")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Collisions")]
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float collisionOffset = 0.2f;

    [Header("Aiming")]
    [SerializeField] private Vector3 aimOffset = new Vector3(0, 1.5f, -1f);
    [SerializeField] private float aimFOV = 40f;
    [SerializeField] private float normalFOV = 60f;

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private Vector3 smoothedPosition;
    private Camera cam;
    private bool isAiming = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        smoothedPosition = target.position + offset;

        currentRotationY = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleCameraRotation();
        HandleAiming();
        UpdateCameraPosition();
        UpdateFOV();
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentRotationY += mouseX;
        currentRotationX -= mouseY;
        currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);
    }

    private void HandleAiming()
    {
        isAiming = Input.GetMouseButton(1);
    }

    private void UpdateCameraPosition()
    {
        Vector3 currentOffset = isAiming ? aimOffset : offset;

        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        Vector3 desiredPosition = target.position + rotation * currentOffset;

        desiredPosition = HandleCameraCollision(desiredPosition, target.position);

        smoothedPosition = Vector3.Lerp(smoothedPosition, desiredPosition, followSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        if (isAiming)
        {
            Vector3 lookDirection = target.forward;
            lookDirection.y = 0;
            transform.LookAt(target.position + lookDirection * 5f + Vector3.up * 1.5f);
        }
        else
        {
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
    }

    private Vector3 HandleCameraCollision(Vector3 desiredPosition, Vector3 targetPosition)
    {
        Vector3 direction = desiredPosition - targetPosition;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.SphereCast(targetPosition, collisionRadius, direction.normalized,
            out hit, distance, collisionMask))
        {
            return targetPosition + direction.normalized * (hit.distance - collisionOffset);
        }

        return desiredPosition;
    }

    private void UpdateFOV()
    {
        if (cam == null) return;

        float targetFOV = isAiming ? aimFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, 10f * Time.deltaTime);
    }

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
}