using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_Text highscoreText;

    void Start()
    {
        highscoreText.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void ShowHighscore()
    {
        highscoreText.text = "Highscore: " + GameManager.Highscore.ToString("F2") + "s";
        highscoreText.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
