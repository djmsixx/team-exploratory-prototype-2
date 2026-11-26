
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float player_move_speed = 5.0f;
    public float player_rotation_speed = 360.0f;
    public float player_sprint_speed = 8.0f;
    public float acceleration = 60.0f;
    public float deceleration = 80.0f;

    [Header("Jump Settings")]
    public float player_jump_force = 5.0f;
    public float fall_gravity_multiplier = 1.5f;
    public float rise_gravity_multiplier = 1.3f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Audio Settings")]
    public AudioSource jumpAudio;

    private Rigidbody rb;
    private Vector3 movement;
    private PlayerPhase phase;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isGrounded;

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

        // Remember jump input for a short time.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Allow jumping shortly after leaving a platform.
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
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
        float rate = movement.sqrMagnitude > 0.001f
            ? acceleration
            : deceleration;

        horizontal = Vector3.MoveTowards(
            horizontal,
            targetVelocity,
            rate * Time.fixedDeltaTime
        );

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

        // Handle jumping with coyote time and jump buffering.
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.AddForce(
                Vector3.up * player_jump_force,
                ForceMode.Impulse
            );

            if (jumpAudio != null)
            {
                jumpAudio.Play();
            }

            // Consume both timers so the player cannot jump again.
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            isGrounded = false;
        }

        // Make the player fall faster.
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(
                Physics.gravity * (fall_gravity_multiplier - 1f),
                ForceMode.Acceleration
            );
        }
        // Make the player rise slower.
        else if (rb.linearVelocity.y > 0)
        {
            rb.AddForce(
                Physics.gravity * (rise_gravity_multiplier - 1f),
                ForceMode.Acceleration
            );
        }
    }

    // Detect when the player is touching the ground.
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    // Detect when the player stops touching a collider.
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
