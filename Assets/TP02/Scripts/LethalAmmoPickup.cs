using UnityEngine;

public class LethalAmmoPickup : MonoBehaviour
{
    [SerializeField] private float activeTime = 12f;
    [SerializeField] private float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack playerAttack = other.GetComponent<Attack>();
            if (playerAttack == null)
            {
                playerAttack = other.GetComponentInParent<Attack>();
            }

            if (playerAttack != null)
            {
                playerAttack.ActivateLethalAmmo(activeTime);

                // Mensaje en pantalla para el jugador
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("<color=red>¡MUNICIÓN LETAL ADQUIRIDA!</color>", 2.5f);
                }

                Destroy(gameObject);
            }
        }
    }
}