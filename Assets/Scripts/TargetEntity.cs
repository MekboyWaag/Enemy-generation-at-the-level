using UnityEngine;

public class TargetEntity : MonoBehaviour, ITarget
{
    [Header("Pathfinding")]
    [SerializeField, Tooltip("Точки маршрута")]
    private Transform[] _waypoints;

    [SerializeField, Min(0.1f)] private float _moveSpeed = 3f;
    [SerializeField, Min(0.1f)] private float _waypointThreshold = 0.5f;

    private int _currentWaypointIndex;
    private bool _isFaulted;

    public Vector3 Position => transform.position;

    private void Awake()
    {
        if (_waypoints == null || _waypoints.Length == 0)
        {
            Debug.LogError($"[{nameof(TargetEntity)}] {name}: Маршрут не назначен!", this);
            _isFaulted = true;
        }
    }

    private void Update()
    {
        if (_isFaulted)
        {
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        Transform targetPoint = _waypoints[_currentWaypointIndex];

        if (targetPoint == null)
        {
            Debug.LogWarning($"[{nameof(TargetEntity)}] {name}: Точка маршрута {_currentWaypointIndex} отсутствует (Null)! Пропускаем.", this);
            SelectNextWaypoint();
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            _moveSpeed * Time.deltaTime);

        if (IsWaypointReached(targetPoint.position))
        {
            SelectNextWaypoint();
        }
    }

    private bool IsWaypointReached(Vector3 targetPosition)
    {
        float thresholdSquared = _waypointThreshold * _waypointThreshold;
        float distanceSquared = Vector3.SqrMagnitude(transform.position - targetPosition);

        return distanceSquared <= thresholdSquared;
    }

    private void SelectNextWaypoint()
    {
        _currentWaypointIndex = ++_currentWaypointIndex % _waypoints.Length;
    }
}