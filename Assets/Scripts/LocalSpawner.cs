using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class LocalSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private LocalSpawnerConfig _spawnerConfig;
    [SerializeField] private EnemyConfig _enemyConfig;
    [SerializeField] private TargetEntity _targetEntity;

    private ObjectPool<Enemy> _pool;
    private Coroutine _spawnRoutine;
    private WaitForSeconds _spawnDelay;
    private bool _isInitialized;
    private bool _isFaulted;

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        if (!ValidateDependencies())
        {
            _isFaulted = true;
            return;
        }

        _spawnDelay = new WaitForSeconds(_spawnerConfig.SpawnInterval);

        _pool = new ObjectPool<Enemy>(
            createFunc: CreateEnemy,
            actionOnGet: null,
            actionOnRelease: ReleaseEnemy,
            actionOnDestroy: DestroyEnemy,
            collectionCheck: false,
            defaultCapacity: _spawnerConfig.InitialPoolCapacity,
            maxSize: _spawnerConfig.MaxPoolCapacity
        );

        PopulatePool();

        _isInitialized = true;

        StartSpawning();
    }

    private bool ValidateDependencies()
    {
        if (_spawnerConfig == null || _enemyConfig == null || _targetEntity == null)
        {
            Debug.LogError($"[{nameof(LocalSpawner)}] {name}: Зависимости не назначены! Остановка.", this);
            return false;
        }

        if (_enemyConfig.EnemyPrefab == null)
        {
            Debug.LogError($"[{nameof(LocalSpawner)}] {name}: В конфиге {_enemyConfig.name} не назначен префаб врага!", this);
            return false;
        }

        return true;
    }

    private void PopulatePool()
    {
        var warmUpCache = ListPool<Enemy>.Get();

        for (int i = 0; i < _spawnerConfig.InitialPoolCapacity; i++)
        {
            warmUpCache.Add(_pool.Get());
        }

        foreach (Enemy enemy in warmUpCache)
        {
            _pool.Release(enemy);
        }

        ListPool<Enemy>.Release(warmUpCache);
    }

    private void StartSpawning()
    {
        StopSpawning();
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void StopSpawning()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (isActiveAndEnabled && !_isFaulted)
        {
            yield return _spawnDelay;
            Spawn();
        }
    }

    private void Spawn()
    {
        Enemy enemy = _pool.Get();
        enemy.Initialize(transform.position, _targetEntity, _enemyConfig);
        enemy.gameObject.SetActive(true);
    }

    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(_enemyConfig.EnemyPrefab);

        enemy.gameObject.SetActive(false);
        enemy.LifetimeEnded += OnEnemyLifetimeEnded;

        return enemy;
    }

    private void ReleaseEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnEnemyLifetimeEnded(Enemy enemy)
    {
        if (!_isInitialized)
        {
            DestroyEnemy(enemy);
            return;
        }

        _pool.Release(enemy);
    }

    private void DestroyEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.LifetimeEnded -= OnEnemyLifetimeEnded;
            Destroy(enemy.gameObject);
        }
    }

    private void OnDestroy()
    {
        StopSpawning();
        _isInitialized = false;
        _pool?.Dispose();
    }
}