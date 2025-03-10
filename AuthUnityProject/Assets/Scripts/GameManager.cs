using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float initialGameSpeed = 5f;
    [SerializeField] private float gameSpeedIncrease = 0.1f;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
        
    public float GameSpeed { get; private set; }

    private Jump player;
    private Spawner spawner;

    private float score;

    private void Awake()
    {
        player = FindFirstObjectByType<Jump>();
        spawner = FindFirstObjectByType<Spawner>();
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        NewGame();
    }

    void Update()
    {
        GameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += GameSpeed * Time.deltaTime;

        scoreText.text = ((int)score).ToString("D5");
    }

    public void NewGame(){
        gameOverPanel.SetActive(false);

        enabled = true;

        Obstacles[] obstacles = FindObjectsByType<Obstacles>(FindObjectsSortMode.None);

        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        score = 0f;

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);

        GameSpeed = initialGameSpeed;
    }

    public void GameOver(){
        gameOverPanel.SetActive(true);

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);

        GameSpeed = 0;
        enabled = false;
    }
}
