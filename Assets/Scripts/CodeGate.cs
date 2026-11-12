using UnityEngine;

public class CodeGate : MonoBehaviour
{
    public string correctCode = "070707";

    private string entered = "";
    private string message = "";
    private bool keypadOpen;
    private bool messagePopupOpen;
    private bool correctCodeEntered;
    private playerMovement movement;
    private GUIStyle keypadTitleStyle;
    private GUIStyle keypadEntryStyle;
    private GUIStyle keypadMessageStyle;
    private GUIStyle keypadButtonStyle;
    private GUIStyle popupTitleStyle;
    private GUIStyle popupMessageStyle;
    private GUIStyle popupButtonStyle;

    private const string CorrectCodeMessage = "I have the right code, how come it doesn't work? fuck it lets just phase through";

    void Start()
    {
        MeshRenderer r = GetComponent<MeshRenderer>();
        if (r != null) r.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (keypadOpen || messagePopupOpen || correctCodeEntered) return;

        PlayerPhase phase = collision.collider.GetComponentInParent<PlayerPhase>();
        if (phase == null || phase.IsPhasing) return;

        OpenKeypad(phase);
    }

    void OpenKeypad(PlayerPhase phase)
    {
        keypadOpen = true;
        entered = "";
        message = "";

        movement = phase.GetComponent<playerMovement>();
        if (movement != null) movement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseKeypad()
    {
        keypadOpen = false;
        if (movement != null) movement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Press(string key)
    {
        if (key == "C")
        {
            entered = "";
        }
        else if (key == "OK")
        {
            if (entered == correctCode)
            {
                correctCodeEntered = true;
                keypadOpen = false;
                entered = "";
                messagePopupOpen = true;
            }
            else
            {
                message = "Wrong code";
                entered = "";
            }
        }
        else if (entered.Length < correctCode.Length)
        {
            entered += key;
        }
    }

    void OnGUI()
    {
        EnsureGuiStyles();

        if (messagePopupOpen)
        {
            DrawCorrectCodePopup();
            return;
        }

        if (!keypadOpen) return;

        float w = Mathf.Min(Screen.width * 0.82f, 450f);
        float h = Mathf.Min(Screen.height * 0.9f, 560f);
        Rect box = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);
        GUI.Box(box, GUIContent.none);

        GUI.Label(new Rect(box.x + 16f, box.y + 14f, w - 32f, 42f), "Enter Code", keypadTitleStyle);
        GUI.Label(new Rect(box.x + 16f, box.y + 59f, w - 32f, 44f), new string('*', entered.Length), keypadEntryStyle);
        GUI.Label(new Rect(box.x + 16f, box.y + 103f, w - 32f, 28f), message, keypadMessageStyle);

        string[] keys = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "C", "0", "OK" };
        float margin = 20f;
        float gap = 10f;
        float buttonWidth = (w - margin * 2f - gap * 2f) / 3f;
        float rowGap = 10f;
        float gridY = box.y + 137f;
        float buttonHeight = Mathf.Max(36f, Mathf.Min(70f, (h - 137f - 76f - rowGap * 3f) / 4f));
        for (int i = 0; i < keys.Length; i++)
        {
            Rect r = new Rect(
                box.x + margin + (i % 3) * (buttonWidth + gap),
                gridY + (i / 3) * (buttonHeight + rowGap),
                buttonWidth,
                buttonHeight);
            if (GUI.Button(r, keys[i], keypadButtonStyle)) Press(keys[i]);
        }

        if (GUI.Button(new Rect(box.x + margin, box.y + h - 64f, w - margin * 2f, 46f), "Cancel", keypadButtonStyle))
            CloseKeypad();
    }

    void DrawCorrectCodePopup()
    {
        float w = Mathf.Min(Screen.width * 0.88f, 820f);
        float h = Mathf.Min(Screen.height * 0.62f, 340f);
        Rect box = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);
        GUI.Box(box, GUIContent.none);
        GUI.Label(new Rect(box.x + 24f, box.y + 14f, w - 48f, 48f), "Keypad", popupTitleStyle);
        GUI.Label(new Rect(box.x + 32f, box.y + 66f, w - 64f, h - 142f), CorrectCodeMessage, popupMessageStyle);

        if (GUI.Button(new Rect(box.center.x - 140f, box.y + h - 66f, 280f, 50f), "Continue", popupButtonStyle))
        {
            messagePopupOpen = false;
            if (movement != null) movement.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void EnsureGuiStyles()
    {
        if (keypadTitleStyle != null) return;

        keypadTitleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 30,
            fontStyle = FontStyle.Bold
        };
        keypadEntryStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 28
        };
        keypadMessageStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20
        };
        keypadButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 24,
            fontStyle = FontStyle.Bold
        };
        popupTitleStyle = new GUIStyle(keypadTitleStyle)
        {
            fontSize = 34
        };
        popupMessageStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 26,
            wordWrap = true
        };
        popupButtonStyle = new GUIStyle(keypadButtonStyle)
        {
            fontSize = 22
        };
    }
}