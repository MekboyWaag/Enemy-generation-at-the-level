using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyConfig _config;
    private Vector3 _moveDirection;
    private bool _isReleased;

    private WaitForSeconds _lifetimeDelay;
    private Coroutine _lifetimeRoutine;

    public event Action<Enemy> LifetimeEnded;

    public void Initialize(Vector3 position, Vector3 direction, EnemyConfig config)
    {
        _config = config;
        _moveDirection = direction.normalized;
        _isReleased = false;

        transform.position = position;

        if (_moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_moveDirection);
        }

        if (_lifetimeDelay == null)
        {
            _lifetimeDelay = new WaitForSeconds(_config.MaxLifetime);
        }

        if (_lifetimeRoutine != null) StopCoroutine(_lifetimeRoutine);
        _lifetimeRoutine = StartCoroutine(LifetimeRoutine());
    }

    private void Update()
    {
        if (_isReleased || _config == null)
        {
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        transform.position += _moveDirection * (_config.MoveSpeed * Time.deltaTime);
    }

    private IEnumerator LifetimeRoutine()
    {
        yield return _lifetimeDelay;
        Release();
    }

    private void Release()
    {
        if (_isReleased)
        {
            return;
        }

        if (_lifetimeRoutine != null)
        {
            StopCoroutine(_lifetimeRoutine);
            _lifetimeRoutine = null;
        }

        LifetimeEnded?.Invoke(this);
    }
}