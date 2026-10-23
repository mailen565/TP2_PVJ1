using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Velocidad y Eje de Rotacion")]
    [SerializeField] private float speed = 60f;
    [SerializeField] private Vector3 axis = Vector3.up; // Rota sobre el eje vertical

    void Update()
    {
        transform.Rotate(axis * speed * Time.deltaTime, Space.World);
    }
}