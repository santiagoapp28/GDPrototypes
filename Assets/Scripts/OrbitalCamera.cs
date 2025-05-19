using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Distance")]
    public float minDistance = 3f;
    public float maxDistance = 20f;
    public float zoomSpeed = 5f;
    public float zoomSmoothTime = 0.2f;

    [Header("Rotation")]
    public float xSpeed = 120f;
    public float ySpeed = 80f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    public float rotationSmoothTime = 0.1f;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public float collisionOffset = 0.2f;
    public LayerMask collisionLayers;

    private float currentDistance;
    private float desiredDistance;
    private float distanceVelocity;

    private Vector2 rotation = new Vector2(0, 0);
    private Vector2 currentRotation;
    private Vector2 rotationVelocity;

    private void Start()
    {
        if (!target)
        {
            Debug.LogWarning("OrbitalCamera: No target assigned.");
            enabled = false;
            return;
        }

        rotation.x = transform.eulerAngles.y;
        rotation.y = transform.eulerAngles.x;

        desiredDistance = currentDistance = Vector3.Distance(transform.position, target.position);
    }

    void LateUpdate()
    {
        if (!target) return;

        HandleInput();
        HandleRotation();
        HandleZoom();
        HandleCollision();
    }

    void HandleInput()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            rotation.x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            rotation.y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            rotation.y = Mathf.Clamp(rotation.y, yMinLimit, yMaxLimit);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            desiredDistance -= scroll * zoomSpeed;
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        }
    }

    void HandleRotation()
    {
        currentRotation = Vector2.SmoothDamp(currentRotation, rotation, ref rotationVelocity, rotationSmoothTime);
    }

    void HandleZoom()
    {
        currentDistance = Mathf.SmoothDamp(currentDistance, desiredDistance, ref distanceVelocity, zoomSmoothTime);
    }

    void HandleCollision()
    {
        Quaternion rot = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        Vector3 targetPosition = target.position;
        Vector3 desiredCameraPosition = targetPosition - (rot * Vector3.forward * currentDistance);

        RaycastHit hit;
        Vector3 direction = desiredCameraPosition - targetPosition;
        if (Physics.SphereCast(targetPosition, collisionRadius, direction.normalized, out hit, currentDistance + collisionOffset, collisionLayers))
        {
            float correctedDistance = hit.distance - collisionOffset;
            currentDistance = Mathf.Min(currentDistance, correctedDistance);
            desiredCameraPosition = targetPosition - (rot * Vector3.forward * currentDistance);
        }

        transform.position = desiredCameraPosition;
        transform.rotation = rot;
    }
}
