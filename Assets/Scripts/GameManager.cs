
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawn Settings")]
    public Transform startingSpawnPoint;
    private Transform currentCheckpoint;

    [Header("Death UI")]
    public GameObject deathPanel;

    private GameObject player;
    private bool isDead = false;
    // Allows scripts to check if the player is dead without allowing them to set it.
    public bool IsDead => isDead;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        deathPanel.SetActive(false);
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
        Debug.Log("Checkpoint activated!");
    }

    public void PlayerDied(GameObject deadPlayer)
    {
        if (isDead)
            return;

        isDead = true;
        player = deadPlayer;

        player.SetActive(false);
        deathPanel.SetActive(true);

        // Unlock mouse for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Respawn()
    {
        if (!isDead)
            return;

        Vector3 respawnPosition = currentCheckpoint != null
            ? currentCheckpoint.position
            : startingSpawnPoint.position;

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        player.transform.position = respawnPosition;
        player.SetActive(true);

        deathPanel.SetActive(false);
        isDead = false;

        // Lock mouse again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
