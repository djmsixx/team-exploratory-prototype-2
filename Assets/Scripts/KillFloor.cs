
using UnityEngine;

public class KillFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDied(other.gameObject);
        }
    }
}
