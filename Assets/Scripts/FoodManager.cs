using Unity.VisualScripting;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public GameObject foodPrefab;
    private float foodCount = 0f;

    private float spawnInterval = 5f;
    private float timeSinceLastSpawn = 0f;
    public static float foodSpawnRate = 3f;

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        spawnInterval = 5f / foodSpawnRate;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            CreateFood();
            timeSinceLastSpawn = 0f;
        }
    }

    void CreateFood()
    {
        GameObject food = GameObject.Instantiate(foodPrefab);
        food.transform.position = GetRandomPosition();
        food.tag = "Food";
        food.name = "Food " + foodCount;
        foodCount++;
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-50f, 50f);
        float z = Random.Range(-50f, 50f);
        return new Vector3(x, 0.5f, z);
    }
}
