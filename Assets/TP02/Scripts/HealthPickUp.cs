using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Configuracion de Curacion")]
    [SerializeField] private float healAmount = 25f;
    [SerializeField] private float rotationSpeed = 60f;

    private bool playerInRange = false;
    private HealthSystem targetHealth = null;

    void Update()
    {
        // Giro visual continuo del objeto
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Si el jugador esta en la zona y presiona 'E', consume el botiquin
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Consume();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            targetHealth = other.GetComponent<HealthSystem>();
            Debug.Log("[Botiquin] Presiona 'E' para curarte.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            targetHealth = null;
        }
    }

    private void Consume()
    {
        if (targetHealth != null && !targetHealth.IsDead)
        {
            // Solo cura si la vida no esta al maximo (100)
            if (targetHealth.CurrentHealth < 100f)
            {
                targetHealth.Heal(healAmount);
                Debug.Log($"[Botiquin] Curacion aplicada (+{healAmount}). Salud actual: {targetHealth.CurrentHealth}");
                Destroy(gameObject); //Desaparece del mapa tras usarse
            }
            else
            {
                Debug.Log("[Botiquin] Tu salud ya esta al maximo.");
            }
        }
    }
}