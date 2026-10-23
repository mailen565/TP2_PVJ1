using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    Playing,
    Paused,
    Victory,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("State Settings")]
    [SerializeField] private GameState currentState;
    public GameState CurrentState => currentState;

    [Header("Score Settings (Punto 3 TP2)")]
    [SerializeField] private int currentScore = 0;
    public int CurrentScore => currentScore;

    [Header("Victory Settings")]
    [SerializeField] private EnemyAI[] enemies;
    [SerializeField] private int totalEnemies;
    [SerializeField] private int enemiesRemaining;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (enemies != null && enemies.Length > 0)
        {
            totalEnemies = enemies.Length;
            enemiesRemaining = totalEnemies;
        }

        // Ejecuta el estado inicial sin importar el valor por defecto
        ApplyState(GameState.Start);
    }

    void Update()
    {
        // 1. Manejo en estado START (Espera de tecla para arrancar la partida)
        if (currentState == GameState.Start)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                ChangeState(GameState.Playing);
            }
            return; // No evalúa pausa si aún no se inició la misión
        }

        // 2. Manejo de PAUSA durante la partida (P o Escape)
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            Paused();
        }

        // 3. Reinicio rápido de nivel con R si perdiste o ganaste
        if ((currentState == GameState.GameOver || currentState == GameState.Victory) && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;
        ApplyState(newState);
    }

    // Aplica la lógica del estado e imprime el mensaje obligatorio en consola
    private void ApplyState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Start:
                Time.timeScale = 0f; // Mantiene el mundo congelado hasta que el jugador decida arrancar
                Debug.Log("<color=white><b>[GameManager] ESTADO: INICIO (Start)</b> - Presiona ESPACIO o ENTER para comenzar la misión.</color>");
                break;

            case GameState.Playing:
                Time.timeScale = 1f; // Activa las físicas y el movimiento
                Debug.Log("<color=green><b>[GameManager] ESTADO: JUGANDO (Playing)</b> - Infiltración en curso.</color>");
                break;

            case GameState.Paused:
                Time.timeScale = 0f; // Pausa el tiempo
                Debug.Log("<color=yellow><b>[GameManager] ESTADO: PAUSA (Paused)</b> - Juego detenido (Time.timeScale = 0).</color>");
                break;

            case GameState.Victory:
                Time.timeScale = 0f;
                Debug.Log($"<color=cyan><b>[GameManager] ESTADO: VICTORIA (Victory)</b> - Misión cumplida con éxito! Puntos obtenidos: {currentScore}</color>");
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                Debug.Log("<color=red><b>[GameManager] ESTADO: GAME OVER (GameOver)</b> - Agente neutralizado. Presiona 'R' para reiniciar.</color>");
                break;
        }
    }

    public void Paused()
    {
        if (currentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
        }
        else if (currentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log($"<color=orange>[GameManager] +{amount} Puntos. Total acumulado: {currentScore}</color>");
    }

    public void NotifyPlayerDead()
    {
        if (currentState != GameState.Playing) return;
        ChangeState(GameState.GameOver);
    }

    public void NotifyEnemyDefeat()
    {
        if (currentState != GameState.Playing) return;

        AddScore(50);
        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            ChangeState(GameState.Victory);
        }
    }

    public void NotifyGoalZone()
    {
        if (currentState != GameState.Playing) return;

        AddScore(100);
        ChangeState(GameState.Victory);
    }
}