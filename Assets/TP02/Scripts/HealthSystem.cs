using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f; 
    private float currentHealth; 
    public float CurrentHealth => currentHealth;//usamos getter y setter para el gameManager - solo lectura

    [SerializeField] private bool isDead;
    public bool IsDead
    {
        get => isDead;
          
    }

    [SerializeField] private GameObject vfx;

    private Rigidbody rb;
    private Collider col;
    private Attack attack;
    private MeshRenderer mesh;
    private void Awake() 
    { 
        currentHealth = maxHealth; 
        isDead = false; 

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        attack = GetComponent<Attack>();
        mesh = GetComponent<MeshRenderer>();
    }
    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        //clamp permite normalizar un valor de acuerdo a los limites
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        /*
         * math.clamp(5, 1, 10) => 5 (esta dentro del rango)
         * math.clamp(-4, 0, 10) => 0 (es menor que el minimo, devuelve 0)
         * math.clamp(15, 0, 10) => 10 (es mayor que el maximo, devuelve 10)
         */

        //Debug.Log($"Salud actual: " + currentHealth);
        Debug.Log($"Salud actual: {currentHealth}");
        if (currentHealth <= 0f)
        {
            Dead();
        }
    }
    private void Dead() 
    { 
        isDead = true;

        //agregar condicional comparando tags player y character, notificar al gameManager

        if (gameObject.CompareTag("Player"))
        {
            GameManager.Instance.NotifyPlayerDead();
        }
        else if(gameObject.CompareTag("Character"))
        {
            GameManager.Instance.NotifyEnemyDefeat();
        }

        vfx.SetActive(true);
        //activar vfx
        StartCoroutine(DestroyTransition());
        //iniciar Corrutina


        Debug.Log("Character eliminado"); 
        // Notificación al GameManager si existiera 
        //GameManager.Instance.ChangeState(GameState.GameOver);
        //Destroy object
        //efectos
        //animaciones
    }

    IEnumerator DestroyTransition()
    {
        col.enabled = false;
        rb.isKinematic = true;
        mesh.enabled = false;
        attack.enabled = false;

        yield return new WaitForSeconds(5f);

        Destroy(this.gameObject);
    }
    //crear corrutina deshabilitando componentes y destruyendo el objeto

}
