using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Tracks whether the game is playing or over, increases score over time,
// and restarts the level a short delay after the player collides.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    enum GameState { Playing, GameOver }

    [SerializeField] float scorePerSecond = 1f;
    [SerializeField] float restartDelay = 2f;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text gameOverText;

    GameState gameState;
    float score;

    public bool IsGameOver => gameState == GameState.GameOver;
    public static float Highscore => PlayerPrefs.GetFloat("HighscoreSeconds", 0f);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        score = 0f;
        gameState = GameState.Playing;
        gameOverText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameState != GameState.Playing) return;

        score += scorePerSecond * Time.deltaTime;
        scoreText.text = "Time: " + score.ToString("F2") + "s";
    }

    public void OnPlayerCollidedWithObstacle()
    {
        if (gameState == GameState.GameOver) return;

        gameState = GameState.GameOver;

        if (score > Highscore)
        {
            PlayerPrefs.SetFloat("HighscoreSeconds", score);
            PlayerPrefs.Save();
        }
        gameOverText.text = "Game Over\nTime: " + score.ToString("F2") + "s\nHighscore: " + Highscore.ToString("F2") + "s";
        gameOverText.gameObject.SetActive(true);

        Invoke(nameof(Restart), restartDelay);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
