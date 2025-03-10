using UnityEngine;

public class Obstacles : MonoBehaviour
{
    private GameManager gameManager;
    private float leftEdge;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 20f;
    }

    void Update()
    {
        transform.position += Vector3.left * gameManager.GameSpeed * Time.deltaTime;

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
