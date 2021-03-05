using System;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class StrikerView : MonoBehaviour
    {
        private Rigidbody2D _rigidBody;
        private Transform _transform;
        public IObservable<Collision2D> CollisionEnter2D { get; private set; }
        private CircleCollider2D _circleCollider;

        private void Awake()
        {
            _transform = transform;
            _circleCollider = GetComponent<CircleCollider2D>();
        }

        public void Configure(PhysicsMaterial2D strikerPhysicsMaterial, Vector2 position)
        {
            _rigidBody = gameObject.AddComponent<Rigidbody2D>();
            _rigidBody.mass = 1;
            _rigidBody.gravityScale = 0;
            _rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rigidBody.sharedMaterial = strikerPhysicsMaterial;
            _transform.position = position;
            _rigidBody.isKinematic = true;
            _circleCollider.enabled = false;
            CollisionEnter2D = gameObject.OnCollisionEnter2DAsObservable();
        }

        public void Strike(Vector2 target)
        {
            _circleCollider.enabled = true;
            _circleCollider.radius = _circleCollider.radius / 2;
            _rigidBody.isKinematic = false;
            _rigidBody.AddForce(target.normalized * 20, ForceMode2D.Impulse);
        }

        public void SetName(string objectName) => _transform.name = objectName;

        public void DestroyComponent()
        {
            Destroy(_rigidBody);
            Destroy(this);
        }

        public void ResetCollider() => _circleCollider.radius = 0.5f;
    }
}