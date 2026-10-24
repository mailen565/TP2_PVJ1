using UnityEngine;

public class MudZone : MonoBehaviour
{
    [SerializeField] private float slowMultiplier = 0.4f; // Reduce la velocidad al 40%
    private float normalSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                normalSpeed = pm.Speed;
                pm.Speed = normalSpeed * slowMultiplier;
                Debug.Log("<color=brown>[Lodo] Velocidad reducida por el fango.</color>");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.Speed = normalSpeed;
                Debug.Log("<color=green>[Lodo] Velocidad restaurada.</color>");
            }
        }
    }
}