using System.Collections;
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
    [SerializeField] private float _spawnDelay = 5f;
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

        StartCoroutine(DelayedSpawnHelicopter(_spawnDelay));
    }

    private void SpawnHelicopter()
    {
        // ランダムで左右に配置
        bool spawnFromLeft = Random.value > 0.5f;
        Vector3 spawnPosition = spawnFromLeft ? _helicopterSpawnPoints.LeftSpawnPoint.position : _helicopterSpawnPoints.RightSpawnPoint.position;

        // ランダムヘリコプター
        GameObject helicopterPrefab = _helicopterPrefabs[Random.Range(0, _helicopterPrefabs.Length)];

        // ヘリコプター生成
        spawnPosition.z = -1;  // 最前面のレイヤーに表示されるようにする
        GameObject helicopter = Instantiate(helicopterPrefab, spawnPosition, Quaternion.identity);

        // 回転を設定する
        Quaternion rot = Quaternion.Euler(0, spawnFromLeft ? 0 : 180, 0);
        helicopter.transform.rotation = rot;

        // コントローラーと体力を取得する
        bool hasController = helicopter.TryGetComponent(out HelicopterController helicopterController);
        bool hasHealth = helicopter.TryGetComponent(out HelicopterHealth helicopterHealth);

        if (!hasController || !hasHealth)
        {
            Debug.LogError("HelicopterSpawner: Spawned helicopter has no HelicopterController or HelicopterHealth!");
            Destroy(helicopter);
            return;
        }

        helicopterController.PlayerTransform = _playerTransform;

        // 破壊されたら新しいヘリコプターをスポーンする
        helicopterHealth.OnHelicopterDestroyed += () => StartCoroutine(DelayedSpawnHelicopter(_spawnDelay));
    }

    private IEnumerator DelayedSpawnHelicopter(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnHelicopter();
    }
}