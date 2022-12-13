using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public GameObject inGamePanel;
    public GameObject finishPanel;
    public GameObject startPanel;
    public GameObject exitPanel;

    public Text userData;
    public Text winLoseText;

    public bool isGameFinished = false;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 0;
        inGamePanel.SetActive(false);
        finishPanel.SetActive(false);
        startPanel.SetActive(true);
        exitPanel.SetActive(false);
    }

    private void Start()
    {
        userData.text = "Total win: " + PlayerPrefs.GetInt("Win") + "\nTotal lose: " + PlayerPrefs.GetInt("Lose");
    }

    public void StartGame()
    {
        isGameFinished = false;
        inGamePanel.SetActive(true);
        finishPanel.SetActive(false);
        startPanel.SetActive(false);
        exitPanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResumeGame()
    {
        StartGame();
    }

    public void PauseGame()
    {
        if (isGameFinished) return;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
        inGamePanel.SetActive(false);
        finishPanel.SetActive(false);
        startPanel.SetActive(false);
        exitPanel.SetActive(true);
    }

    public void ShowFinalPanel(bool isWin)
    {
        isGameFinished = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;

        winLoseText.color = isWin ? Color.green : Color.red;
        winLoseText.text = isWin ? "You won" : "Game over";

        inGamePanel.SetActive(false);
        finishPanel.SetActive(true);
        startPanel.SetActive(false);
        exitPanel.SetActive(false);
    }
}