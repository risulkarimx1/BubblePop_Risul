using System;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

public class ExplosionController : MonoBehaviour, IPoolable<Vector3, IMemoryPool>, IDisposable
{
    private Animator _animator;
    private Transform _transform;
    private static readonly int _explode = Animator.StringToHash("Explode");

    private Vector3 _position;
    private IMemoryPool _pool;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
    }

    public void OnDespawned()
    {
        _pool = null;
        _position = Constants.HiddenPosition;
    }

    public void OnSpawned(Vector3 position, IMemoryPool memoryPool)
    {
        _transform.position = position;
        _pool = memoryPool;
        _animator.SetTrigger(_explode);
    }

    public void Dispose()
    {
        _pool.Despawn(this);
    }

    public class Factory : PlaceholderFactory<Vector3, ExplosionController>
    {
    }
}
