using UnityEngine;

public class ControlsHud : MonoBehaviour
{
    private Texture2D pixel;
    private GUIStyle titleStyle;
    private GUIStyle keyStyle;
    private GUIStyle actionStyle;

    private void Awake()
    {
        pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();
    }

    private void OnDestroy()
    {
        if (pixel != null)
            Destroy(pixel);
    }

    private void OnGUI()
    {
        EnsureStyles();

        float scale = Mathf.Clamp(Mathf.Min(Screen.width / 1600f, Screen.height / 900f), 0.7f, 1.1f);
        titleStyle.fontSize = Mathf.RoundToInt(16f * scale);
        keyStyle.fontSize = Mathf.RoundToInt(17f * scale);
        actionStyle.fontSize = Mathf.RoundToInt(13f * scale);
        float panelWidth = 270f * scale;
        float panelHeight = 270f * scale;
        float margin = 20f * scale;
        Rect panel = new Rect(margin, margin, panelWidth, panelHeight);

        DrawRect(new Rect(panel.x + 3f * scale, panel.y + 4f * scale, panel.width, panel.height), new Color(0f, 0f, 0f, 0.28f));
        DrawRect(panel, new Color(0.055f, 0.075f, 0.1f, 0.84f));
        GUI.Label(new Rect(panel.x + 14f * scale, panel.y + 8f * scale, panel.width - 28f * scale, 28f * scale), "CONTROLS", titleStyle);

        float keySize = 36f * scale;
        float gap = 4f * scale;
        float keysX = panel.x + 18f * scale;
        float keysY = panel.y + 42f * scale;
        DrawKey(new Rect(keysX + keySize + gap, keysY, keySize, keySize), "W", Input.GetKey(KeyCode.W));
        DrawKey(new Rect(keysX, keysY + keySize + gap, keySize, keySize), "A", Input.GetKey(KeyCode.A));
        DrawKey(new Rect(keysX + keySize + gap, keysY + keySize + gap, keySize, keySize), "S", Input.GetKey(KeyCode.S));
        DrawKey(new Rect(keysX + (keySize + gap) * 2f, keysY + keySize + gap, keySize, keySize), "D", Input.GetKey(KeyCode.D));
        float actionX = keysX + (keySize + gap) * 3f + 14f * scale;
        float actionWidth = panel.xMax - actionX - 12f * scale;
        DrawAction("MOVE", actionX, keysY, 2f * keySize + gap, actionWidth);

        float lowerRowY = keysY + (keySize + gap) * 2f + 12f * scale;
        DrawKey(new Rect(keysX, lowerRowY, keySize * 2.45f, keySize), "SPACE", Input.GetKey(KeyCode.Space));
        DrawAction("JUMP", actionX, lowerRowY, keySize, actionWidth);

        float mouseY = lowerRowY + keySize + 7f * scale;
        DrawKey(new Rect(keysX, mouseY, keySize * 2.45f, keySize), "RMB", Input.GetMouseButton(1));
        DrawAction("RIGHT CLICK\nPHASE", actionX, mouseY, keySize, actionWidth);

        float sprintY = mouseY + keySize + 7f * scale;
        DrawKey(new Rect(keysX, sprintY, keySize * 2.45f, keySize), "SHIFT", Input.GetKey(KeyCode.LeftShift));
        DrawAction("SPRINT", actionX, sprintY, keySize, actionWidth);
    }

    private void DrawAction(string label, float x, float y, float height, float width)
    {
        GUI.Label(new Rect(x, y, width, height), label, actionStyle);
    }

    private void DrawKey(Rect rect, string label, bool isPressed)
    {
        Color edgeColor = isPressed ? new Color(0.18f, 0.78f, 0.72f, 1f) : new Color(0.36f, 0.43f, 0.51f, 1f);
        Color fillColor = isPressed ? new Color(0.08f, 0.39f, 0.39f, 1f) : new Color(0.12f, 0.16f, 0.22f, 1f);

        DrawRect(rect, edgeColor);
        float inset = Mathf.Max(2f, rect.width * 0.07f);
        DrawRect(new Rect(rect.x + inset, rect.y + inset, rect.width - inset * 2f, rect.height - inset * 2f), fillColor);
        GUI.Label(rect, label, keyStyle);
    }

    private void DrawRect(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, pixel);
        GUI.color = previous;
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 16,
            fontStyle = FontStyle.Bold
        };
        SetTextColor(titleStyle, new Color(0.7f, 0.83f, 0.9f));

        keyStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 17,
            fontStyle = FontStyle.Bold
        };
        SetTextColor(keyStyle, Color.white);

        actionStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 13,
            fontStyle = FontStyle.Bold
        };
        SetTextColor(actionStyle, new Color(0.89f, 0.93f, 0.96f));
    }

    private static void SetTextColor(GUIStyle style, Color color)
    {
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;
        style.focused.textColor = color;
    }
}
