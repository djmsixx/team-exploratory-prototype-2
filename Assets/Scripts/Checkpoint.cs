
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;
    private ParticleSystem particles;

    private void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated)
            return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;

            GameManager.Instance.SetCheckpoint(transform);

            if (particles != null)
            {
                particles.Play();
            }

            Debug.Log("Checkpoint activated!");
        }
    }
}
