using UnityEngine;

public class EnemyCoordinator : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField, Tooltip("Массив всех автономных спавнеров на сцене")]
    private LocalSpawner[] _spawners;

    private void Start()
    {
        if (_spawners == null || _spawners.Length == 0)
        {
            Debug.LogWarning($"[{nameof(EnemyCoordinator)}] {name}: Локальные спавнеры не назначены.", this);
            return;
        }

        for (int i = 0; i < _spawners.Length; i++)
        {
            LocalSpawner spawner = _spawners[i];

            if (spawner == null)
            {
                Debug.LogWarning($"[{nameof(EnemyCoordinator)}] {name}: Слот спавнера с индексом {i} пуст (Null)! Пропускаем.", this);
                continue;
            }

            spawner.Initialize();
        }
    }
}