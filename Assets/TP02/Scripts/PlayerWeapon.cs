using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Ajustes de Disparo")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 50f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private Transform firePoint; // Cañón del arma o Main Camera
    [SerializeField] private LayerMask hitLayers; // Filtrar por capas (Interactable / Enemy / Default)

    [Header("Efecto de Aturdimiento (Opcional A)")]
    [SerializeField] private bool stunEnemy = true;
    [SerializeField] private float stunDuration = 2f;

    private float nextFireTime = 0f;

    void Update()
    {
        // No disparar si el juego está en Pausa, Inicio o GameOver
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        // Clic izquierdo para disparar
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 origin = (firePoint != null) ? firePoint.position : Camera.main.transform.position;
        Vector3 direction = (firePoint != null) ? firePoint.forward : Camera.main.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, range, hitLayers))
        {
            Debug.Log($"[PlayerWeapon] Impacto en: {hit.collider.name}");

            // Quitar vida al enemigo
            HealthSystem enemyHealth = hit.collider.GetComponentInParent<HealthSystem>();
            if (enemyHealth != null && hit.collider.CompareTag("Character"))
            {
                enemyHealth.TakeDamage(damage);
            }

            // Aplicar aturdimiento
            if (stunEnemy)
            {
                EnemyAI enemyAI = hit.collider.GetComponentInParent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Stun(stunDuration);
                }
            }
        }

        Debug.DrawRay(origin, direction * range, Color.cyan, 0.25f);
    }
}