using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField, Tooltip("Точки спавна со скриптом SpawnPoint.")]
    private SpawnPoint[] _spawnPoints;

    private EnemySpawnerConfig _config;
    private EnemyConfig _elementConfig;
    private ObjectPool<Enemy> _pool;

    private Coroutine _spawnRoutine;
    private WaitForSeconds _spawnDelay;
    private bool _isInitialized;

    private void OnDestroy()
    {
        StopSpawning();

        _isInitialized = false;

        _pool?.Dispose();
    }

    public void Initialize(EnemySpawnerConfig config, EnemyConfig elementConfig)
    {
        if (_isInitialized)
        {
            return;
        }

        _config = config;
        _elementConfig = elementConfig;

        if (_config.EnemyPrefab == null || _spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogError("[EnemySpawner] Dependencies or Spawn Points missing!", this);
            return;
        }

        _spawnDelay = new WaitForSeconds(_config.SpawnInterval);

        _pool = new ObjectPool<Enemy>(
            createFunc: CreateElement,
            actionOnGet: element => element.gameObject.SetActive(true),
            actionOnRelease: element => element.gameObject.SetActive(false),
            actionOnDestroy: DestroyElement,
            collectionCheck: false,
            defaultCapacity: _config.InitialPoolCapacity,
            maxSize: _config.MaxPoolCapacity
        );

        PopulatePool();

        _isInitialized = true;

        StartSpawning();
    }

    private void PopulatePool()
    {
        var warmUpCache = ListPool<Enemy>.Get();

        for (int i = 0; i < _config.InitialPoolCapacity; i++)
        {
            warmUpCache.Add(_pool.Get());
        }

        foreach (var element in warmUpCache)
        {
            _pool.Release(element);
        }

        ListPool<Enemy>.Release(warmUpCache);
    }

    private void StartSpawning()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
        }

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
        while (isActiveAndEnabled)
        {
            yield return _spawnDelay;
            Spawn();
        }
    }

    private void Spawn()
    {
        Enemy element = _pool.Get();
        SpawnPoint spawnPoint = GetRandomSpawnPoint();

        element.Initialize(spawnPoint.Position, spawnPoint.Direction, _elementConfig);
    }

    private Enemy CreateElement()
    {
        Enemy element = Instantiate(_config.EnemyPrefab, transform);

        element.gameObject.SetActive(false);
        element.LifetimeEnded += OnEnemyLifetimeEnded;

        return element;
    }

    private void OnEnemyLifetimeEnded(Enemy element)
    {
        if (!_isInitialized)
        {
            Destroy(element.gameObject);
            return;
        }

        _pool.Release(element);
    }

    private void DestroyElement(Enemy element)
    {
        if (element != null)
        {
            element.LifetimeEnded -= OnEnemyLifetimeEnded;
            Destroy(element.gameObject);
        }
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[randomIndex];
    }
}