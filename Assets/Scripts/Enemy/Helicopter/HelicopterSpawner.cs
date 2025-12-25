using UnityEngine;

[System.Serializable]
struct HelicopterSpawnPoints
{
    public Transform LeftSpawnPoint;
    public Transform RightSpawnPoint;
}

public class HelicopterSpawner : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private HelicopterSpawnPoints _helicopterSpawnPoints;
    [SerializeField] private GameObject[] _helicopterPrefabs;
    #endregion

    #region Properties
    private Transform _playerTransform;
    #endregion

    void Start()
    {
        _playerTransform = FindAnyObjectByType<PlayerController>().transform;

        if (!_playerTransform)
        {
            Debug.LogError("HelicopterSpawner: No PlayerController found in scene!");
            enabled = false;
            return;
        }

        if (!_helicopterSpawnPoints.LeftSpawnPoint || !_helicopterSpawnPoints.RightSpawnPoint)
        {
            Debug.LogError("HelicopterSpawner: Helicopter spawn points not assigned!");
            enabled = false;
            return;
        }

        SpawnHelicopter();
    }

    private void SpawnHelicopter()
    {
        // random spawn position
        bool spawnFromLeft = Random.value > 0.5f;
        Vector3 spawnPosition = spawnFromLeft ? _helicopterSpawnPoints.LeftSpawnPoint.position : _helicopterSpawnPoints.RightSpawnPoint.position;

        // random helicopter
        GameObject helicopterPrefab = _helicopterPrefabs[Random.Range(0, _helicopterPrefabs.Length)];

        // spawn helicopter
        spawnPosition.z = -1;  // ensure in front most layer
        GameObject helicopter = Instantiate(helicopterPrefab, spawnPosition, Quaternion.identity);

        // set rotation
        Quaternion rot = Quaternion.Euler(0, spawnFromLeft ? 0 : 180, 0);
        helicopter.transform.rotation = rot;

        // get controller and health
        bool hasController = helicopter.TryGetComponent(out HelicopterController helicopterController);
        bool hasHealth = helicopter.TryGetComponent(out HelicopterHealth helicopterHealth);

        if (!hasController || !hasHealth)
        {
            Debug.LogError("HelicopterSpawner: Spawned helicopter has no HelicopterController or HelicopterHealth!");
            Destroy(helicopter);
            return;
        }

        helicopterController.PlayerTransform = _playerTransform;

        // destroy new helicopter when destroyed
        helicopterHealth.OnHelicopterDestroyed += SpawnHelicopter;
    }
}