using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;

    [SerializeField] private bool isDead;
    public bool IsDead => isDead;

    [SerializeField] private GameObject vfx;

    private Rigidbody rb;
    private Collider col;
    private Attack attack;
    private Renderer rend;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        attack = GetComponent<Attack>();
        
        // Busca cualquier Renderer (MeshRenderer o SkinnedMeshRenderer) en el objeto o sus hijos sin dar error
        rend = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        // Inicializa la barra de vida en la UI al comenzar si es el Player
        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Actualiza el slider y texto del HUD
        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log($"[HealthSystem - {gameObject.name}] Salud actual: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Dead();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Actualiza el slider y texto del HUD al curarse
        if (gameObject.CompareTag("Player") && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log($"[HealthSystem - {gameObject.name}] Salud recuperada: +{amount}. Salud actual: {currentHealth}");
    }

    private void Dead()
    {
        if (isDead) return;
        isDead = true;

        if (gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null) GameManager.Instance.NotifyPlayerDead();
        }
        else if (gameObject.CompareTag("Character"))
        {
            if (GameManager.Instance != null) GameManager.Instance.NotifyEnemyDefeat();
        }

        if (vfx != null)
        {
            vfx.SetActive(true);
        }

        StartCoroutine(DestroyTransition());
        Debug.Log($"[HealthSystem] {gameObject.name} eliminado.");
    }

    private IEnumerator DestroyTransition()
    {
        // Desactiva físicas para que no siga colisionando tras morir
        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true;

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}