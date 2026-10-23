
using System.Collections;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private float openDuration = 3f;

    private Camera playerCamera;

    private void Start()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit,
            interactionDistance, interactableLayer))
        { 
            Debug.Log("¡Puerta detectada!");

            Animator doorAnimator =
                hit.collider.GetComponentInParent<Animator>();

            Debug.Log("Animator encontrado: " + (doorAnimator != null));

            if (doorAnimator != null)
            {
                if (!doorAnimator.GetBool("EstaAbierta"))
                {
                    StartCoroutine(OpenAndCloseDoor(doorAnimator));
                }
            }
            else
            {
                Debug.Log("El objeto detectado no tiene Animator.");
            }
        }
    }

    private IEnumerator OpenAndCloseDoor(Animator doorAnimator)
    {
        doorAnimator.SetBool("EstaAbierta", true);
        doorAnimator.SetTrigger("Abrir");

        Debug.Log("Orden de abrir enviada al Animator");

        yield return new WaitForSeconds(openDuration);

        doorAnimator.SetBool("EstaAbierta", false);
    }
}
