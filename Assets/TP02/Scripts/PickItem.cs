using UnityEngine;

public class PickItem : MonoBehaviour
{
    [SerializeField] private Transform hand;

    private GameObject currentItem = null;
    private GameObject itemInRange = null; // Guarda el ítem con el que estamos haciendo contacto

    void Update()
    {
        // El Update corre a 60+ FPS y nunca pierde el toque de la tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentItem == null && itemInRange != null)
            {
                Pick(itemInRange);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Al entrar en el área del ítem, lo dejamos listo para recoger
        if (other.CompareTag("Item"))
        {
            itemInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si nos alejamos del ítem sin agarrarlo, limpiamos la referencia
        if (other.CompareTag("Item") && itemInRange == other.gameObject)
        {
            itemInRange = null;
        }
    }

    private void Pick(GameObject item)
{
    currentItem = item;

    // Apaga el giro para que quede firme en la mano
    Rotator rot = item.GetComponent<Rotator>();
    if (rot != null) rot.enabled = false;

    if (hand != null)
    {
        item.transform.SetParent(hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    Rigidbody rb = item.GetComponent<Rigidbody>();
    Collider col = item.GetComponent<Collider>();

    if (rb != null) rb.isKinematic = true;
    if (col != null) col.enabled = false;
}

    public GameObject DropItem()
    {
        GameObject temp = currentItem;
        currentItem = null;
        return temp;
    }
}