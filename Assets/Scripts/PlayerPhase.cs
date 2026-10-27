using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerPhase : MonoBehaviour
{
    [Tooltip("0 = left click, 1 = right click, 2 = middle click")]
    public int phaseMouseButton = 1;
    public string phaseableLayerName = "Phaseable";
    public float slowMultiplier = 0.4f;
    public float slowSmoothing = 10f;
    public Renderer playerRenderer;
    public Color phasedColor = new Color(0.4f, 0.8f, 1f);
    public bool IsPhasing { get; private set; }
    public bool InsideWall { get; private set; }
    public float SpeedMultiplier { get; private set; } = 1f;
    private CapsuleCollider capsule;
    private int phaseableLayer;
    private int phaseableMask;
    private bool wantsPhase;
    private Color normalColor;
    private readonly Collider[] hits = new Collider[8];

    void Start()
    {
        capsule = GetComponent<CapsuleCollider>();
        phaseableLayer = LayerMask.NameToLayer(phaseableLayerName);

        if (phaseableLayer < 0)
        {
            Debug.LogError($"Layer '{phaseableLayerName}' doesn't exist. Add it in Project Settings > Tags and Layers.");
            enabled = false;
            return;
        }

        phaseableMask = 1 << phaseableLayer;

        if (playerRenderer == null) playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null) normalColor = playerRenderer.material.color;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(phaseMouseButton))
            wantsPhase = !wantsPhase;

        InsideWall = IsOverlappingPhaseable();

        if (wantsPhase && !IsPhasing)
        {
            SetPhasing(true);
        }
        else if (!wantsPhase && IsPhasing && !InsideWall)
        {
            SetPhasing(false);
        }

        float target = (IsPhasing && InsideWall) ? slowMultiplier : 1f;
        SpeedMultiplier = Mathf.Lerp(SpeedMultiplier, target, 1f - Mathf.Exp(-slowSmoothing * Time.deltaTime));
    }

    void SetPhasing(bool on)
    {
        IsPhasing = on;
        Physics.IgnoreLayerCollision(gameObject.layer, phaseableLayer, on);

        if (playerRenderer != null)
            playerRenderer.material.color = on ? phasedColor : normalColor;
    }

    bool IsOverlappingPhaseable()
    {
        float radius = capsule.radius * 0.9f;
        float halfSegment = Mathf.Max(0f, capsule.height * 0.5f - capsule.radius);
        Vector3 center = transform.TransformPoint(capsule.center);
        Vector3 p1 = center + Vector3.up * halfSegment;
        Vector3 p2 = center - Vector3.up * halfSegment;

        int count = Physics.OverlapCapsuleNonAlloc(
            p1, p2, radius, hits, phaseableMask, QueryTriggerInteraction.Ignore);

        return count > 0;
    }

    void OnDisable()
    {
            Physics.IgnoreLayerCollision(gameObject.layer, phaseableLayer, false);
    }
}