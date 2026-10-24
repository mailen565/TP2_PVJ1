using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Paneles de Estados")]
    [SerializeField] private GameObject panelStart;
    [SerializeField] private GameObject panelHUD;
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelVictory;

    [Header("Elementos del HUD")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI ammoModeText;

    [Header("Notificaciones en Pantalla")]
    [SerializeField] private TextMeshProUGUI notificationText;
    private Coroutine notificationCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowStartScreen()
    {
        SetActivePanel(panelStart);
    }

    public void ShowHUD()
    {
        SetActivePanel(panelHUD);
    }

    public void ShowPauseScreen()
    {
        if (panelPause != null) panelPause.SetActive(true);
    }

    public void HidePauseScreen()
    {
        if (panelPause != null) panelPause.SetActive(false);
    }

    public void ShowGameOver()
    {
        SetActivePanel(panelGameOver);
    }

    public void ShowVictory()
    {
        SetActivePanel(panelVictory);
    }

    private void SetActivePanel(GameObject panelToActivate)
    {
        if (panelStart != null) panelStart.SetActive(panelStart == panelToActivate);
        if (panelHUD != null) panelHUD.SetActive(panelHUD == panelToActivate);
        if (panelPause != null) panelPause.SetActive(panelPause == panelToActivate);
        if (panelGameOver != null) panelGameOver.SetActive(panelGameOver == panelToActivate);
        if (panelVictory != null) panelVictory.SetActive(panelVictory == panelToActivate);
    }

    // Actualiza la barra y el texto de salud
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"SALUD: {Mathf.CeilToInt(currentHealth)} / {maxHealth}";
        }
    }

    // Actualiza el texto del modo de munición
    public void UpdateAmmoStatus(bool isLethal)
    {
        if (ammoModeText != null)
        {
            ammoModeText.text = isLethal 
                ? "<color=red>MUNICIÓN: LETAL (DESTRUCCIÓN)</color>" 
                : "<color=yellow>MUNICIÓN: DARDOS PEM (ATURDIR)</color>";
        }
    }

    // Muestra un mensaje temporal en el HUD y lo apaga tras unos segundos
    public void ShowNotification(string message, float duration = 2.5f)
    {
        if (notificationText == null) return;

        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationCoroutine = StartCoroutine(NotificationRoutine(message, duration));
    }

    private IEnumerator NotificationRoutine(string message, float duration)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        notificationText.text = "";
        notificationText.gameObject.SetActive(false);
    }
}