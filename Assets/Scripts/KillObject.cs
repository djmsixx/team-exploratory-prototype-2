
using UnityEngine;

public class KillObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDied(other.gameObject);
        }
    }
}
