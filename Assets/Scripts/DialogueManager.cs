
using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public float displayDuration = 4f;

    private Coroutine dialogueCoroutine;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string text)
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        dialogueText.text = text;
        dialoguePanel.SetActive(true);

        dialogueCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        dialoguePanel.SetActive(false);
        dialogueCoroutine = null;
    }
}
