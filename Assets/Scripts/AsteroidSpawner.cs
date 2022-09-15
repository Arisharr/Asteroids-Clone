using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public Asteroids[] asteroidPrefab;
    public float spawnRate = 4f;
    public float spawnAmt = 2f;
    public float spawnDistance = 15f;
    public float spawnRot = 15f;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    private void Spawn()
    {
        for (int i = 0; i < spawnAmt; i++)
        { 
            Vector3 spawnPose, randomRot, spawnDir;

            spawnDir = Random.insideUnitCircle.normalized * spawnDistance;
            spawnPose = transform.position + spawnDir;

            float _var = Random.Range(-spawnRot, spawnRot);
            Quaternion rotation = Quaternion.AngleAxis(_var, Vector3.forward);

            int _index = Random.Range(0, asteroidPrefab.Length);
            Asteroids asteroid = Instantiate(asteroidPrefab[_index], spawnPose, rotation);
            asteroid.SetOnFire(rotation * -spawnDir);
        }
    }
}
