using UnityEngine;

public class Scroll : MonoBehaviour
{
    private GameManager gameManager;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        float speed = gameManager.GameSpeed / transform.localScale.x;
        meshRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
    }
}
