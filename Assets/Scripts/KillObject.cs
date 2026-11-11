
using UnityEngine;

public class KillObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPhase phase = other.GetComponent<PlayerPhase>();

            if (phase != null && phase.IsPhasing)
            {
                return;
            }

            GameManager.Instance.PlayerDied(other.gameObject);
        }
    }
}
