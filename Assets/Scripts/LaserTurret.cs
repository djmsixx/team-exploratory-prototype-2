using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Degrees per second. Negative spins the other way.")]
    public float rotationSpeed = 40f;

    [Header("Nozzles")]
    public Transform[] nozzles;

    [Header("Beam")]
    public bool laserOn = true;
    public float maxLength = 30f;
    public float beamWidth = 0.15f;
    public Color beamColor = Color.red;
    [Tooltip("Optional. If empty, a red URP Unlit material is made automatically.")]
    public Material beamMaterial;
    public bool keepBeamsHorizontal = true;

    [Header("Collision")]
    [Tooltip("Layers that STOP the beam. The player's own layer is always included automatically.")]
    public LayerMask blockMask;
    public PlayerPhase player;   // auto-found if left empty

    private LineRenderer[] lines;
    private int playerMask;
    private readonly RaycastHit[] hitBuffer = new RaycastHit[32];
    private static readonly Comparer<RaycastHit> byDistance =
        Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance));

    void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerPhase>();
        if (player == null)
        {
            Debug.LogError("LaserTurret couldn't find a PlayerPhase in the scene.");
            enabled = false;
            return;
        }

        // whatever layer the player is actually on (Default is fine)
        playerMask = 1 << player.gameObject.layer;

        Material mat = beamMaterial;
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader != null)
            {
                mat = new Material(shader);
                mat.color = beamColor;
            }
            else
            {
                Debug.LogWarning("Couldn't find URP Unlit shader. Assign a Beam Material.");
            }
        }

        lines = new LineRenderer[nozzles.Length];
        for (int i = 0; i < nozzles.Length; i++)
        {
            LineRenderer lr = nozzles[i].GetComponent<LineRenderer>();
            if (lr == null) lr = nozzles[i].gameObject.AddComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.startWidth = beamWidth;
            lr.endWidth = beamWidth;
            lr.numCapVertices = 2;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            if (mat != null) lr.material = mat;
            lines[i] = lr;
        }
    }

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

        int mask = blockMask | playerMask;

        for (int i = 0; i < nozzles.Length; i++)
        {
            LineRenderer lr = lines[i];
            lr.enabled = laserOn;
            if (!laserOn) continue;

            Vector3 origin = nozzles[i].position;
            Vector3 dir = origin - transform.position;
            if (keepBeamsHorizontal) dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) dir = nozzles[i].forward;
            dir.Normalize();

            Vector3 end = origin + dir * maxLength;

            int count = Physics.RaycastNonAlloc(origin, dir, hitBuffer, maxLength, mask,
                                                QueryTriggerInteraction.Ignore);
            System.Array.Sort(hitBuffer, 0, count, byDistance);

            for (int h = 0; h < count; h++)
            {
                RaycastHit hit = hitBuffer[h];
                bool isPlayer = hit.collider.GetComponentInParent<PlayerPhase>() == player;

                if (isPlayer)
                {
                    if (player.IsPhasing) continue;   // phased: beam passes straight through

                    end = hit.point;
                    KillPlayer();
                    break;
                }

                end = hit.point;   // wall, glass, etc.
                break;
            }

            lr.SetPosition(0, origin);
            lr.SetPosition(1, end);
        }
    }

    void KillPlayer()
    {
        if (GameManager.Instance == null) return;

        // PlayerDied ignores repeat calls while the player is already dead
        GameManager.Instance.PlayerDied(player.gameObject);
    }
}