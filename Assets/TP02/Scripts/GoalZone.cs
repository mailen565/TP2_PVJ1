using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [SerializeField] private Transform goal;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickItem pickItem = other.GetComponent<PickItem>();

            if(pickItem!=null)
            {
                GameObject item = pickItem.DropItem();

                if(item!=null)
                {
                    //meta -> Victoria
                    ItemInZone(item);

                    Debug.Log("Completaste el nivel");
                }
                else
                {
                    Debug.Log("Derrota o LLego a la meta pero sin el item");
                    //Derrota o LLego a la meta pero sin el item
                }
            }
        }
    }


    private void ItemInZone(GameObject item)
    {
        item.transform.SetParent(goal);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Collider col = item.GetComponent<Collider>();
        Rigidbody rb = item.GetComponent<Rigidbody>();

        if(col!=null) col.enabled = true;
        if(rb!=null) rb.isKinematic = false;



    }
}
