using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainMenuCanvas;
    public GameObject timerUI;
    public GameObject pauseMenuCanvas;
    public Button startButton;
    public Button quitButton;
    public Button resumeButton;
    public Button returnToMenuButton;
    public TMP_Text timerText;

    private float timer;
    private bool timerRunning = false;
    private bool isPaused = false;

    public static bool hasStarted = false;

    void Start()
    {
        pauseMenuCanvas.SetActive(false);

        if (!hasStarted)
        {
            ShowMenu();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            mainMenuCanvas.SetActive(false);
            timerUI.SetActive(true);
            timerRunning = true;
        }

        // Boutons pause
        resumeButton.onClick.AddListener(ResumeGame);
        returnToMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        if (timerRunning && !isPaused)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("F2") + "s";
        }

        if (hasStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    void ShowMenu()
    {
        mainMenuCanvas.SetActive(true);
        timerUI.SetActive(false);
        timer = 0f;

        startButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();

        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        hasStarted = true;
        mainMenuCanvas.SetActive(false);
        timerUI.SetActive(true);
        timerRunning = true;
        Time.timeScale = 1f;
    }

    void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void PauseGame()
    {
        isPaused = true;
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void ReturnToMainMenu()
    {
        hasStarted = false;
        Time.timeScale = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
