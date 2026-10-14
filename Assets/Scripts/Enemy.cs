using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const float MinRotationDistanceSquared = 0.001f;
    private EnemyConfig _config;
    private ITarget _target;

    private float _remainingLifetime;
    private bool _isReleased;

    public event Action<Enemy> LifetimeEnded;

    public void Initialize(Vector3 spawnPosition, ITarget target, EnemyConfig config)
    {
        _config = config;
        _target = target;
        _remainingLifetime = _config.MaxLifetime;
        _isReleased = false;

        transform.position = spawnPosition;

        if (_target != null)
        {
            UpdateRotation(_target.Position);
        }
    }

    private void Update()
    {
        if (_isReleased || _config == null || _target == null)
        {
            return;
        }

        UpdateLifetime();
        HandleMovement();
    }

    private void UpdateLifetime()
    {
        _remainingLifetime -= Time.deltaTime;

        if (_remainingLifetime <= 0f)
        {
            Release();
        }
    }

    private void HandleMovement()
    {
        Vector3 targetPosition = _target.Position;

        UpdateRotation(targetPosition);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            _config.MoveSpeed * Time.deltaTime);
    }

    private void UpdateRotation(Vector3 targetPosition)
    {
        Vector3 offset = targetPosition - transform.position;

        if (Vector3.SqrMagnitude(offset) > MinRotationDistanceSquared)
        {
            transform.rotation = Quaternion.LookRotation(offset.normalized);
        }
    }

    private void Release()
    {
        if (_isReleased)
        {
            return;
        }

        _isReleased = true;
        LifetimeEnded?.Invoke(this);
    }

    private void OnDestroy()
    {
        LifetimeEnded = null;
    }
}