using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Speed")]
    [SerializeField] private float initialGameSpeed = 5f;
    [SerializeField] private float gameSpeedIncrease = 0.1f;

    [Header("In Game UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Scoreboard")]
    [SerializeField] private GameObject scoreBoardPanel;
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private GameObject leaderboardEntryPrefab;

    [Header("Log In")]
    [SerializeField] private GameObject logInPanel;
        
    public float GameSpeed { get; private set; }

    private Jump player;
    private Spawner spawner;

    private float score;

    private AuthHandler authHandler;

    private void Awake()
    {
        player = FindFirstObjectByType<Jump>();
        spawner = FindFirstObjectByType<Spawner>();
        authHandler = FindFirstObjectByType<AuthHandler>();
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        scoreBoardPanel.SetActive(false);
        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);        
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
        authHandler.UpdateScoreFunction((int)score);

        gameOverPanel.SetActive(true);

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);

        GameSpeed = 0;
        enabled = false;
    }

    public void ExitGame(){
        logInPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void ShowScoreboard(){
        authHandler.StartCoroutine(authHandler.GetUsers());
        scoreBoardPanel.SetActive(true);
    }

    public void HideScoreboard(){
        scoreBoardPanel.SetActive(false);
    }

    public void UpdateScoreboard(UserModel[] leaderboard)
    {
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var user in leaderboard)
        {
            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = user.username;
            texts[1].text = user.data.score.ToString();
        }
    }
}
