using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Attack : MonoBehaviour
{
    [Header("Raycast Shoot Settings (Arma de Fuego)")]
    [SerializeField] private Transform gun;
    [SerializeField] private float rayDistance = 50f;
    [SerializeField] private Vector3 offsetGun;
    [SerializeField] private float shotDuration = 0.07f;

    [Header("Melee Settings (Cuerpo a Cuerpo)")]
    [SerializeField] private Transform sword;
    [SerializeField] private float radius = 2f;
    [SerializeField] private Vector3 offsetSword = new Vector3(0, 1f, 0.5f);

    [Header("Attack Settings")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float cooldownAttack = 0.5f;

    [SerializeField] private bool isPlayer;
    [SerializeField] private bool isMelee;

    [Header("Mecánica Aturdir / Modo Letal")]
    [SerializeField] private bool isLethalMode = false; // False = aturde | True = destruye
    [SerializeField] private float stunDuration = 3f;   // Segundos de parálisis

    private bool canAttack = true;
    private Vector3 origin;
    private Vector3 dir;
    private bool isCharacter;
    private RaycastHit hit;
    private PlayerMovement pM;
    private LineRenderer laserLine;

    private void Awake()
    {
        pM = GetComponent<PlayerMovement>();
        laserLine = GetComponent<LineRenderer>();

        if (laserLine != null)
        {
            laserLine.enabled = false;
        }
    }

    private void Update()
    {
        if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
            {
                if (isMelee)
                {
                    Melee();
                }
                else
                {
                    Shoot();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!isMelee)
        {
            if (hit.collider == null) return;
            Gizmos.color = isCharacter ? Color.green : Color.red;
            Gizmos.DrawLine(origin, hit.point);
        }
        else
        {
            Gizmos.color = isCharacter ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin, radius);
        }
    }

    public void Shoot()
    {
        RaycastInfo(offsetGun, false);

        Vector3 endPosition;

        if (Physics.Raycast(origin, dir, out hit, rayDistance))
        {
            endPosition = hit.point;
            Debug.Log($"[Attack] Disparo impactó en: {hit.collider.name}");
            AplyDamage(hit.collider.gameObject);
        }
        else
        {
            endPosition = origin + (dir * rayDistance);
        }

        if (laserLine != null)
        {
            StartCoroutine(ShotEffect(origin, endPosition));
        }

        if (isPlayer)
        {
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator ShotEffect(Vector3 start, Vector3 end)
    {
        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
        laserLine.enabled = true;

        yield return new WaitForSeconds(shotDuration);

        laserLine.enabled = false;
    }

    public void Melee()
    {
        RaycastInfo(offsetSword, true);

        Collider[] colliders = Physics.OverlapSphere(origin, radius);
        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if (col.gameObject == gameObject) continue;

                if (col.gameObject.CompareTag("Character") || col.gameObject.CompareTag("Player"))
                {
                    AplyDamage(col.gameObject);
                }
            }
        }
        else
        {
            isCharacter = false;
        }

        if (isPlayer)
        {
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        float currentSpeed = 0f;

        if (pM != null)
        {
            currentSpeed = pM.Speed;
            pM.Speed = 0f;
        }

        yield return new WaitForSeconds(cooldownAttack);

        if (pM != null)
        {
            pM.Speed = currentSpeed;
        }
        canAttack = true;
    }

    private void AplyDamage(GameObject hitObj)
    {
        if (hitObj == null) return;

        // Ataque cuerpo a cuerpo del robot hacia el jugador
        if (!isPlayer)
        {
            HealthSystem playerHealth = hitObj.GetComponentInParent<HealthSystem>();
            if (playerHealth != null && hitObj.CompareTag("Player"))
            {
                playerHealth.TakeDamage(damage);
                isCharacter = true;
            }
            return;
        }

        // Disparo del jugador hacia el enemigo
        if (hitObj.CompareTag("Character"))
        {
            isCharacter = true;
            EnemyAI enemy = hitObj.GetComponentInParent<EnemyAI>();

            if (isLethalMode)
            {
                // MODO LETAL (Power-Up recogido): causa daño y destruye
                HealthSystem targetHealth = hitObj.GetComponentInParent<HealthSystem>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);
                }
            }
            else
            {
                // MODO BASE (Sin Power-Up): aturde temporalmente
                if (enemy != null)
                {
                    Debug.Log($"<color=yellow>[Attack] Enemigo aturdido por {stunDuration} segundos.</color>");
                    enemy.Stun(stunDuration);
                }
            }
        }
        else
        {
            isCharacter = false;
        }
    }

    // Método que llama el Power-Up para activar la munición destructiva
    public void ActivateLethalAmmo(float duration)
    {
        StartCoroutine(LethalAmmoRoutine(duration));
    }

    private IEnumerator LethalAmmoRoutine(float duration)
    {
        isLethalMode = true;
        Debug.Log("<color=red><b>[POWER-UP] Modo Letal activado. Ahora destruyes enemigos.</b></color>");

        yield return new WaitForSeconds(duration);

        isLethalMode = false;
        Debug.Log("<color=yellow>[POWER-UP] Modo Letal terminado. Volviendo a aturdimiento.</color>");
    }

    private void RaycastInfo(Vector3 offset, bool useSword)
    {
        if (useSword)
        {
            origin = (sword != null) ? sword.TransformPoint(offset) : transform.TransformPoint(offset);
            dir = transform.forward;
        }
        else
        {
            origin = (gun != null) ? gun.TransformPoint(offset) : (Camera.main != null ? Camera.main.transform.position : transform.position);
            dir = (Camera.main != null) ? Camera.main.transform.forward : transform.forward;
        }
    }
}