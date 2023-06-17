using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2.0f;

    [Header("movement")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float height = 2.0f;

    private float startingLocalY;

    void Start()
    {
        //movement
        startingLocalY = transform.localPosition.y;

        //spawning
        StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {
        SpawnerMovement();
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnerMovement()
    {
        float newY = Mathf.Sin(Time.time * speed) * height + startingLocalY;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }
}
