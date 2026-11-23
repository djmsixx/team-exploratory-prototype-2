
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float player_move_speed = 5.0f;
    public float player_rotation_speed = 360.0f;
    public float player_jump_force = 5.0f;
    public float player_sprint_speed = 8.0f;
    public float acceleration = 60.0f;
    public float deceleration = 80.0f;

    private Rigidbody rb;
    private Vector3 movement;
    private bool jumpRequested;
    private PlayerPhase phase;
    public AudioSource jumpAudio;

    void Start()
    {
        phase = GetComponent<PlayerPhase>();
        rb = GetComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontalInput, 0, verticalInput);
        input = Vector3.ClampMagnitude(input, 1.0f);

        Transform cameraTransform = Camera.main != null
            ? Camera.main.transform
            : null;

        Vector3 forward = cameraTransform != null
            ? cameraTransform.forward
            : Vector3.forward;

        Vector3 right = cameraTransform != null
            ? cameraTransform.right
            : Vector3.right;

        forward.y = 0.0f;
        right.y = 0.0f;

        forward.Normalize();
        right.Normalize();

        movement = (right * input.x) + (forward * input.z);

        if (Input.GetKeyDown(KeyCode.Space) &&
            Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            jumpRequested = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            player_move_speed = player_sprint_speed;
        }
        else
        {
            player_move_speed = 5.0f;
        }
    }

    void FixedUpdate()
    {
        float phaseSlow = phase != null
            ? phase.SpeedMultiplier
            : 1f;

        // Apply horizontal movement while preserving vertical velocity.
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontal = new Vector3(velocity.x, 0, velocity.z);

        Vector3 targetVelocity = movement * player_move_speed * phaseSlow;

        // Speed up when there's input, slow down faster when there isn't.
        float rate = movement.sqrMagnitude > 0.001f ? acceleration : deceleration;

        horizontal = Vector3.MoveTowards(horizontal, targetVelocity, rate * Time.fixedDeltaTime);

        velocity.x = horizontal.x;
        velocity.z = horizontal.z;
        rb.linearVelocity = velocity;

        // Rotate player toward movement direction.
        if (movement.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            Quaternion rotation = Quaternion.RotateTowards(
                rb.rotation,
                targetRotation,
                player_rotation_speed * Time.fixedDeltaTime
            );

            rb.MoveRotation(rotation);
        }

        // Handle jumping.
        if (jumpRequested)
        {
            rb.AddForce(
                Vector3.up * player_jump_force,
                ForceMode.Impulse
            );

            if (jumpAudio != null)
            {
                jumpAudio.Play();
            }

            jumpRequested = false;
        }
    }
}
