using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalPickup : MonoBehaviour
{
    [SerializeField] private string endMessage = "You Win!";

    private bool gameEnded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gameEnded) return;
        if (!other.CompareTag("Player")) return;

        gameEnded = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnGUI()
    {
        if (!gameEnded) return;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 48;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(0, Screen.height / 2 - 80, Screen.width, 100), endMessage, titleStyle);

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 40, 200, 50), "Restart"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}