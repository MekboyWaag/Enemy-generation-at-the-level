using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyConfig _config;
    private Vector3 _moveDirection;

    private bool _isReleased;
    private float _lifetimeTimer;

    public event Action<Enemy> OnLifetimeEnded;

    public void Initialize(Vector3 position, Vector3 direction, EnemyConfig config)
    {
        _config = config;
        _moveDirection = direction.normalized;

        _isReleased = false;
        _lifetimeTimer = 0f;

        transform.position = position;

        if (_moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_moveDirection);
        }
    }

    private void Update()
    {
        if (_isReleased || _config == null)
        {
            return;
        }

        HandleMovement();
        HandleLifetime();
    }

    private void HandleMovement()
    {
        transform.position += _moveDirection * (_config.MoveSpeed * Time.deltaTime);
    }

    private void HandleLifetime()
    {
        _lifetimeTimer += Time.deltaTime;

        if (_lifetimeTimer >= _config.MaxLifetime)
        {
            Release();
        }
    }

    private void OnDestroy()
    {
        OnLifetimeEnded = null;
    }

    private void Release()
    {
        if (_isReleased)
        {
            return;
        }

        _isReleased = true;
        OnLifetimeEnded?.Invoke(this);
    }
}