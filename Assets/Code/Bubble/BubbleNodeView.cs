using System;
using DG.Tweening;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeView : MonoBehaviour
    {
        private Transform _transform;
        private PhysicsMaterial2D _strikerPhysicsMaterial;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        public void SetPosition(Vector2 position, bool animate)
        {
            if (animate)
            {
                _transform.DOMove(position, 1,false);
            }
            else
            {
                _transform.position = position;
            }

        }

        public void SetPosition(Coordinate coordinate)
        {
            var colPosition = coordinate.Col * 1.0f;
            if (coordinate.Row % 2 == 1)
            {
                colPosition -= 0.5f;
            }

            SetPosition(new Vector2(colPosition, -coordinate.Row), false);
        }

        public StrikerView ConvertToStriker()
        {
            return gameObject.AddComponent<StrikerView>();
        }

        public void AddPhysicsMaterial(PhysicsMaterial2D strikerPhysicsMaterial)
        {
            _strikerPhysicsMaterial = strikerPhysicsMaterial;
        }
    }
}