using System;
using Assets.Code.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class StrikerView : MonoBehaviour
    {
        private Rigidbody2D _rigidBody;
        private Transform _transform;
        public IObservable<Collision2D> CollisionEnter2D { get; private set; }

        private void Awake()
        {
            _transform = transform;
        }

        public void Configure(PhysicsMaterial2D strikerPhysicsMaterial, Vector2 position)
        {
            _rigidBody = gameObject.AddComponent<Rigidbody2D>();
            _rigidBody.mass = 1;
            _rigidBody.gravityScale = 0;
            _rigidBody.sharedMaterial = strikerPhysicsMaterial;
            _transform.position = position;
            _rigidBody.isKinematic = true;
            CollisionEnter2D = gameObject.OnCollisionEnter2DAsObservable();
        }

        public void Strike(Vector2 target)
        {
            _rigidBody.isKinematic = false;
            _rigidBody.AddForce((target - (Vector2) transform.position).normalized * 10, ForceMode2D.Impulse);
        }

        public void SetName(string objectName)
        {
            _transform.name = objectName;
        }

        public void DestroyComponent()
        {
            Destroy(_rigidBody);
            Destroy(this);
        }
    }
}