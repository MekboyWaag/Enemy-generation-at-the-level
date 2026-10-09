using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Enemy _prefab;
    [SerializeField, Tooltip("Точки, из которых могут появляться враги. Направление задается осью Z точки.")]
    private Transform[] _spawnPoints;

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

        if (_prefab == null || _spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogError("[EnemySpawner] Dependencies or Spawn Points missing!", this);
            return;
        }

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (_spawnPoints[i] == null)
            {
                Debug.LogError($"[EnemySpawner] Spawn point at index {i} is NULL! Fix in Inspector.", this);
                return;
            }
        }

        _config = config;
        _elementConfig = elementConfig;

        _spawnDelay = new WaitForSeconds(_config.SpawnInterval);

        _pool = new ObjectPool<Enemy>(
            createFunc: CreateElement,
            actionOnGet: null,
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
        while (true)
        {
            yield return _spawnDelay;
            Spawn();
        }
    }

    private void Spawn()
    {
        Enemy element = _pool.Get();
        Transform spawnPoint = GetRandomSpawnPoint();

        element.Initialize(spawnPoint.position, spawnPoint.forward, _elementConfig);
        element.gameObject.SetActive(true);
    }

    private Enemy CreateElement()
    {
        Enemy element = Instantiate(_prefab, transform);

        element.gameObject.SetActive(false);
        element.OnLifetimeEnded += ReturnToPool;

        return element;
    }

    private void ReturnToPool(Enemy element)
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
            element.OnLifetimeEnded -= ReturnToPool;
            Destroy(element.gameObject);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[randomIndex];
    }
}