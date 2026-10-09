using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Systems/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    private const float SpeedWarningThreshold = 100f;

    [Header("Movement Settings")]
    [SerializeField, Min(0.1f)] private float _moveSpeed = 5f;

    [Header("Lifecycle Settings")]
    [SerializeField, Tooltip("Время жизни для защиты от утечек (секунды)")]
    [Min(1f)] private float _maxLifetime = 10f;

    public float MoveSpeed => _moveSpeed;
    public float MaxLifetime => _maxLifetime;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_moveSpeed > SpeedWarningThreshold)
        {
            Debug.LogWarning($"[EnemyConfig] Скорость превышает рекомендованный лимит ({SpeedWarningThreshold})!");
        }
    }
#endif
}