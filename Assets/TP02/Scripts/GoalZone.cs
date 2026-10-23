using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [Header("Referencia del pedestal/soporte")]
    [SerializeField] private Transform goal;

    private bool completed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (completed) return;

        if (other.CompareTag("Player"))
        {
            // Busca PickItem en el Player o en sus componentes superiores
            PickItem pickItem = other.GetComponent<PickItem>();
            if (pickItem == null)
            {
                pickItem = other.GetComponentInParent<PickItem>();
            }

            if (pickItem != null)
            {
                GameObject item = pickItem.DropItem();

                if (item != null)
                {
                    completed = true;
                    ItemInZone(item);

                    Debug.Log("<color=green>[GoalZone] Objeto de datos depositado correctamente.</color>");

                    // Notifica al GameManager para sumar puntos y cambiar a estado Victory
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.NotifyGoalZone();
                    }
                }
                else
                {
                    Debug.Log("<color=yellow>[GoalZone] Llegaste a la meta, pero no traes ningún ítem equipado.</color>");
                }
            }
        }
    }

    private void ItemInZone(GameObject item)
    {
        Transform targetTransform = (goal != null) ? goal : transform;

        item.transform.SetParent(targetTransform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Collider col = item.GetComponent<Collider>();
        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = true;
    }
}