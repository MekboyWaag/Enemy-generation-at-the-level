using UnityEngine;

[CreateAssetMenu(fileName = "LocalSpawnerConfig", menuName = "Systems/Local Spawner Config")]
public class LocalSpawnerConfig : ScriptableObject
{
    private const int DefaultSafetyBufferMultiplier = 2;

    [Header("Pool Settings")]
    [SerializeField, Min(1)] private int _initialPoolCapacity = 15;

    [SerializeField, Tooltip("Жесткий лимит памяти.")]
    [Min(1)] private int _maxPoolCapacity = 30;

    [SerializeField, Min(0.1f)] private float _spawnInterval = 2f;

    public int InitialPoolCapacity => _initialPoolCapacity;
    public int MaxPoolCapacity => _maxPoolCapacity;
    public float SpawnInterval => _spawnInterval;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_maxPoolCapacity < _initialPoolCapacity)
        {
            _maxPoolCapacity = _initialPoolCapacity * DefaultSafetyBufferMultiplier;

            Debug.LogWarning(
                $"[{nameof(LocalSpawnerConfig)}] {name}: MaxPoolCapacity не может быть меньше InitialPoolCapacity. Установлен безопасный буфер (x{DefaultSafetyBufferMultiplier}).",
                this);
        }
    }
#endif
}