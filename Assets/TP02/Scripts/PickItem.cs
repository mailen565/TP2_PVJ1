using UnityEngine;

public class PickItem : MonoBehaviour
{

    [SerializeField] private Transform hand;

    private GameObject currentItem = null;

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Item") && currentItem == null)
        {

            if(Input.GetKeyDown(KeyCode.E)) Pick(other.gameObject);

        }

    }


    private void Pick(GameObject item)
    {
        currentItem = item;

        item.transform.SetParent(hand);//error null reference
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        //verificaciones para evitar conflictos con el jugador
        Rigidbody rb = item.GetComponent<Rigidbody>();

        Collider collider = item.GetComponent<Collider>();

        if(rb!=null) rb.isKinematic = true;
        if(collider!= null) collider.enabled = false;
    }

    public GameObject DropItem()
    {
        GameObject temp = currentItem;
        currentItem = null;
        return temp;
    }

 

}
