using UnityEngine;

public class CodeGate : MonoBehaviour
{
    public string correctCode = "070707";

    private string entered = "";
    private string message = "";
    private bool keypadOpen;
    private bool unlocked;
    private playerMovement movement;

    void Start()
    {
        MeshRenderer r = GetComponent<MeshRenderer>();
        if (r != null) r.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (unlocked || keypadOpen) return;

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
                unlocked = true;
                GetComponent<Collider>().enabled = false;
                CloseKeypad();
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
        if (!keypadOpen) return;

        float w = 220f, h = 330f;
        Rect box = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);
        GUI.Box(box, "Enter Code");

        GUI.Label(new Rect(box.x + 10, box.y + 25, w - 20, 22), new string('*', entered.Length));
        GUI.Label(new Rect(box.x + 10, box.y + 45, w - 20, 22), message);

        string[] keys = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "C", "0", "OK" };
        for (int i = 0; i < keys.Length; i++)
        {
            Rect r = new Rect(box.x + 10 + (i % 3) * 70, box.y + 70 + (i / 3) * 50, 65, 45);
            if (GUI.Button(r, keys[i])) Press(keys[i]);
        }

        if (GUI.Button(new Rect(box.x + 10, box.y + h - 45, w - 20, 35), "Cancel"))
            CloseKeypad();
    }
}