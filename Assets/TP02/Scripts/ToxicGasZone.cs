using UnityEngine;

public class ToxicGasZone : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 8f;
    private float timer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer += Time.deltaTime;
            if (timer >= 1f) // Aplica daño cada 1 segundo
            {
                HealthSystem hp = other.GetComponent<HealthSystem>();
                if (hp == null) hp = other.GetComponentInParent<HealthSystem>();

                if (hp != null)
                {
                    hp.TakeDamage(damagePerSecond);
                }
                timer = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0f;
        }
    }
}