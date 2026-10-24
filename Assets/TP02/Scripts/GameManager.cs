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
            
            Time.timeScale = 0f;
            if (UIManager.Instance != null) UIManager.Instance.ShowStartScreen();
            break;
            
            case GameState.Playing:
            Time.timeScale = 1f;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowHUD();
                UIManager.Instance.HidePauseScreen();
            }
            break;
            
            case GameState.Paused:
            Time.timeScale = 0f;
            if (UIManager.Instance != null) UIManager.Instance.ShowPauseScreen();
            break;
            
            case GameState.GameOver:
            Time.timeScale = 0f;
            if (UIManager.Instance != null) UIManager.Instance.ShowGameOver();
            break;
            
            case GameState.Victory:
            Time.timeScale = 0f;
            if (UIManager.Instance != null) UIManager.Instance.ShowVictory();
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

        AddScore(50); // Suma 50 puntos por eliminar la amenaza
        enemiesRemaining--;

        Debug.Log($"<color=cyan>[GameManager] Guardia neutralizado. Enemigos restantes: {enemiesRemaining}</color>");

    }

    public void NotifyGoalZone()
    {
        if (currentState != GameState.Playing) return;

        AddScore(100);
        ChangeState(GameState.Victory);
    }
}