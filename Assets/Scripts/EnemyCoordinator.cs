using UnityEngine;

public class EnemyCoordinator : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] private EnemySpawnerConfig _spawnerConfig;
    [SerializeField] private EnemyConfig _enemyConfig;

    [Header("Systems")]
    [SerializeField] private EnemySpawner _spawner;

    private void Start()
    {
        if (_spawnerConfig == null || _enemyConfig == null || _spawner == null)
        {
            Debug.LogError("[EnemyCoordinator] Dependencies missing! Initialization aborted.", this);
            return;
        }

        _spawner.Initialize(_spawnerConfig, _enemyConfig);
    }
}