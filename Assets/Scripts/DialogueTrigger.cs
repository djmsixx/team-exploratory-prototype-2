
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(3, 5)]
    public string dialogue;

    private bool hasTriggered;
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        PlayerPhase player = other.GetComponentInParent<PlayerPhase>();

        if (player == null)
            return;

        if (dialogueManager == null)
            return;

        hasTriggered = true;
        dialogueManager.ShowDialogue(dialogue);
    }
}
