using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] ParticleSystem explosion;
    [SerializeField] AudioSource mainTheme;
    [SerializeField] AudioSource audioSource;
    [SerializeField] int lives = 3;
    [SerializeField] float respawnDelay = 3f;
    [SerializeField] float respawnCooldown = 3f;
    [Space]
    public int score;
    public int highScore;
    [Space]
    [SerializeField] TMP_Text score_text;
    [SerializeField] TMP_Text highScore_text;
    [SerializeField] TMP_Text newHighScore_text;
    [SerializeField] GameObject gameOver_panel;
    [SerializeField] Image[] lives_img;
    [SerializeField] AudioClip gameOver_clip;
    [SerializeField] AudioClip playerDeath_clip;
    [SerializeField] AudioClip asteroidDeath_clip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore");

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        ScoreManager();
        UIManager();
    }

    public void Spawn()
    {
        Player.Instance.isAlive = true;
        Player.Instance.startCooldown = respawnCooldown;
        Player.Instance.transform.position = Vector3.zero;

        Player.Instance.GetComponent<SpriteRenderer>().enabled = true;
        Player.Instance.idleParticle.gameObject.SetActive(true);
        mainTheme.volume = .8f;
    }

    public void Respawn()
    {
        explosion.transform.position = Player.Instance.transform.position;
        explosion.Play();
        audioSource.PlayOneShot(playerDeath_clip);
        lives--;
        if (lives <= 0) GameOver();
        else Invoke(nameof(Spawn), respawnDelay);
    }

    public void GameOver()
    {
        audioSource.PlayOneShot(gameOver_clip);
        gameOver_panel.SetActive(true);
        HandleTheme(100000f);
    }

    private void HandleTheme(float _dur)
    {
        float elapsed = 0f;

        while (elapsed < _dur)
        {
            mainTheme.pitch = Mathf.Lerp(1f, .5f, elapsed / _dur);
            elapsed += Time.deltaTime;
        }

        mainTheme.pitch = .5f;
    }

    public void DestroyAsteroid(Asteroids asteroid)
    {
        explosion.transform.position = asteroid.transform.position;
        explosion.Play();
        audioSource.PlayOneShot(asteroidDeath_clip);
        score++;
    }

    private void ScoreManager()
    {
        if (score>= highScore)
        {
            newHighScore_text.gameObject.SetActive(true);
            highScore = score;
        }

        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    private void UIManager()
    {
        score_text.text = score.ToString();
        highScore_text.text = highScore.ToString();

        switch (lives)
        {
            case 2:
                lives_img[2].enabled = false;
                break;
            case 1:
                lives_img[1].enabled = false;
                break;
            case 0:
                lives_img[0].enabled = false;
                break;
            default:
                break;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
