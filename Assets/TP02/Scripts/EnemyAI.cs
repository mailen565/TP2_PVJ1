using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack }

    [Header("States")]
    [SerializeField] private AIState currentState = AIState.Patrol;

    [Header("Detect Settings")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackRadius = 2f;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTimeAtPoint = 2f;
    private int currentPatrolIndex;
    private bool isWaitingAtPoint;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    private bool isAttacking = false;

    [Header("Animación")]
    [SerializeField] private string punchTriggerName = "Punch";
    [SerializeField] private string speedFloatName = "Speed";

    private NavMeshAgent agent;
    private Transform playerTransform;
    private HealthSystem healthSystemPlayer;
    private HealthSystem healthSystemSelf;
    private Attack attack;
    private Animator animator;
    private bool isStunned;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<Attack>();
        healthSystemSelf = GetComponent<HealthSystem>();
        
        // Busca el Animator en este objeto o en los hijos visuales del modelo 3D
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            healthSystemPlayer = playerObj.GetComponent<HealthSystem>();
        }
        else
        {
            Debug.LogError("[EnemyAI] No hay GameObjects con tag 'Player'");
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            GoToNextPatrolPoint();
        }
    }

    private void Update()
    {
        // 1. Si el enemigo murió, se frena de inmediato
        if (healthSystemSelf != null && healthSystemSelf.IsDead)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        // 2. Si está aturdido, no hay jugador o el jugador murió, se detiene
        if (isStunned || playerTransform == null || (healthSystemPlayer != null && healthSystemPlayer.IsDead))
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            UpdateAnimationSpeed(0f);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrolState(distanceToPlayer);
                break;

            case AIState.Chase:
                UpdateChaseState(distanceToPlayer);
                break;

            case AIState.Attack:
                UpdateAttackState(distanceToPlayer);
                break;
        }

        // Actualiza el parámetro de caminado del Animator según el movimiento real
        if (agent != null && agent.isOnNavMesh)
        {
            UpdateAnimationSpeed(agent.velocity.magnitude);
        }
    }

    private void UpdatePatrolState(float distanceToPlayer)
    {
        if (agent != null && agent.isOnNavMesh) agent.isStopped = false;

        if (distanceToPlayer <= detectionRadius)
        {
            currentState = AIState.Chase;
            return;
        }

        if (agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaitingAtPoint)
            {
                StartCoroutine(WaitAndMoveToNextPoint());
            }
        }
    }

    private void UpdateChaseState(float distanceToPlayer)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }

        // Transición Ataque
        if (distanceToPlayer <= attackRadius)
        {
            currentState = AIState.Attack;
            return;
        }

        // Si el jugador se escapa del radio de detección, vuelve a patrullar
        if (distanceToPlayer > detectionRadius * 1.2f)
        {
            currentState = AIState.Patrol;
            GoToNextPatrolPoint();
        }
    }

    private void UpdateAttackState(float distanceToPlayer)
    {
        // Rota mirando hacia el jugador sobre el eje horizontal
        Vector3 targetLook = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(targetLook);

        // Transición de regreso a Persecución si el jugador se aleja
        if (distanceToPlayer > attackRadius && !isAttacking)
        {
            currentState = AIState.Chase;
            return;
        }

        if (!isAttacking)
        {
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        isAttacking = true;
        if (agent != null && agent.isOnNavMesh) agent.isStopped = true;

        ExecuteAttack();

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        if (agent != null && agent.isOnNavMesh) agent.isStopped = false;
    }

    private void ExecuteAttack()
    {
        if (healthSystemPlayer != null)
        {
            Debug.Log("[EnemyAI] Ataque contra el jugador");

            // Dispara el trigger de la animación de puño
            if (animator != null && !string.IsNullOrEmpty(punchTriggerName))
            {
                animator.SetTrigger(punchTriggerName);
            }

            // Aplica el daño mediante el componente Attack
            if (attack != null)
            {
                attack.Melee();
            }
        }
    }

    private IEnumerator WaitAndMoveToNextPoint()
    {
        isWaitingAtPoint = true;
        yield return new WaitForSeconds(waitTimeAtPoint);

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            GoToNextPatrolPoint();
        }

        isWaitingAtPoint = false;
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || agent == null || !agent.isOnNavMesh) return;

        if (patrolPoints[currentPatrolIndex] != null)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    private void UpdateAnimationSpeed(float speed)
    {
        if (animator != null && !string.IsNullOrEmpty(speedFloatName))
        {
            animator.SetFloat(speedFloatName, speed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Radio de detección (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Radio de ataque (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    // Opcional A: Aturdimiento
    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
        UpdateAnimationSpeed(0f);
        Debug.Log($"<color=yellow>[EnemyAI] Aturdido por {duration} segundos</color>");

        yield return new WaitForSeconds(duration);

        isStunned = false;
        if (agent != null && agent.isOnNavMesh) agent.isStopped = false;
        Debug.Log("<color=green>[EnemyAI] Recuperado del aturdimiento</color>");
    }
}