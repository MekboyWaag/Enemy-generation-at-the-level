using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "Systems/Enemy Spawner Config")]
public class EnemySpawnerConfig : ScriptableObject
{
    private const int DefaultSafetyBufferMultiplier = 2;

    [Header("Dependencies")]
    [SerializeField, Tooltip("Префаб врага")]
    private Enemy _enemyPrefab;

    [Header("Pool Settings")]
    [SerializeField, Min(5)] private int _initialPoolCapacity = 30;

    [SerializeField, Tooltip("Жесткий лимит памяти.")]
    [Min(5)] private int _maxPoolCapacity = 60;

    [SerializeField, Min(0.1f)] private float _spawnInterval = 2f;

    public Enemy EnemyPrefab => _enemyPrefab;
    public int InitialPoolCapacity => _initialPoolCapacity;
    public int MaxPoolCapacity => _maxPoolCapacity;
    public float SpawnInterval => _spawnInterval;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_maxPoolCapacity < _initialPoolCapacity)
        {
            _maxPoolCapacity = _initialPoolCapacity * DefaultSafetyBufferMultiplier;

            Debug.LogWarning($"[EnemySpawnerConfig] MaxPoolCapacity не может быть меньше Initial. Установлен безопасный буфер (x{DefaultSafetyBufferMultiplier}).");
        }
    }
#endif
}