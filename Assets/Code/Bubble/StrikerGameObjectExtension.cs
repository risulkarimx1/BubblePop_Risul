using UnityEngine;

namespace Assets.Code.Bubble
{
    public static class StrikerGameObjectExtension
    {
        public static StrikerView AddStrikerComponent(this GameObject gameObject, PhysicsMaterial2D physicsMaterial)
        {
            var strikerComponent = gameObject.AddComponent<StrikerView>();
            var rigidBody = gameObject.AddComponent<Rigidbody2D>();
            rigidBody.mass = 1;
            rigidBody.gravityScale = 0;
            rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rigidBody.interpolation = RigidbodyInterpolation2D.None;
            gameObject.GetComponent<CircleCollider2D>().sharedMaterial = physicsMaterial;
            return strikerComponent;
        }
    }
}