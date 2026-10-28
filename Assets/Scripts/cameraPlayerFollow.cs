using UnityEngine;

// Attach this script to the camera object.
public class camera : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 180.0f;
    public float followDistance = 6.0f;
    public float followHeight = 3.0f;
    public float followSmoothTime = 0.08f;
    public float minimumPitch = -30.0f;
    public float maximumPitch = 60.0f;
    public LayerMask collisionMask;
    public float cameraRadius = 0.3f;
    public float minimumDistance = 0.5f;

    private PlayerPhase phase;
    private int phaseableLayer;

    private float yaw;
    private float pitch;

    // public GoalPickup goalPickup;

    void Start()
    {
        if (player == null)
        {
            return;
        }
        phase = player.GetComponent<PlayerPhase>();
        phaseableLayer = LayerMask.NameToLayer("Phaseable");

        Vector3 initialOffset = transform.position - player.position;
        yaw = Mathf.Atan2(-initialOffset.x, -initialOffset.z) * Mathf.Rad2Deg;
        pitch = Mathf.Asin(Mathf.Clamp(initialOffset.y / initialOffset.magnitude, -1.0f, 1.0f)) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // if ((goalPickup == null || !goalPickup.gameEnded) &&
        //     Cursor.lockState != CursorLockMode.Locked &&
        //     Input.GetMouseButtonDown(0))
        // {
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        // }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0.0f);
        Vector3 desiredPosition = player.position + orbitRotation * new Vector3(0.0f, followHeight, -followDistance);
        Vector3 pivot = player.position + Vector3.up * (followHeight * 0.5f);
        Vector3 toCamera = desiredPosition - pivot;
        float distance = toCamera.magnitude;
        Vector3 direction = toCamera / distance;

        int mask = collisionMask;
        if (phase != null && phase.IsPhasing && phaseableLayer >= 0)
        {
            mask &= ~(1 << phaseableLayer);
        }

        if (Physics.SphereCast(pivot, cameraRadius, direction, out RaycastHit hit, distance,
                            mask, QueryTriggerInteraction.Ignore))
        {
            desiredPosition = pivot + direction * Mathf.Max(hit.distance, minimumDistance);
        }

        transform.position = desiredPosition;
        transform.LookAt(player.position + Vector3.up * (followHeight * 0.5f));
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}