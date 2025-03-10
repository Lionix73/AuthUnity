using UnityEngine;

public class Spawner : MonoBehaviour
{   
    [SerializeField] private Spawnable[] spawnables;    
    [SerializeField] private float minSpawnRate = 1f;
    [SerializeField] private float maxSpawnRate = 2f;

    private void OnEnable()
    {
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Spawn(){
        float spawnChance = Random.value;

        foreach (var spawnable in spawnables)
        {
            if (spawnChance < spawnable.weight)
            {
                GameObject obstacle = Instantiate(spawnable.prefab);
                obstacle.transform.position += transform.position;
                break;
            }

            spawnChance -= spawnable.weight;
        }

        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));   
    }

    [System.Serializable]
    public class Spawnable
    {
        public GameObject prefab;
        [Range(0, 1)]public float weight;
    }
}
