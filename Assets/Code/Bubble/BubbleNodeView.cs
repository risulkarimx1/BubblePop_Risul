using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeView : MonoBehaviour
    {
        public List<string> Neighbors; 
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            
        }

        public void SetPosition(Vector2 position, bool animate = false, float speed = 1)
        {
            if (animate)
            {
                _transform.DOMove(position, 1 / speed);
            }
            else
            {
                _transform.position = position;
            }
        }

        public StrikerView ConvertToStriker() => gameObject.AddComponent<StrikerView>();

        public Vector3 GetPosition() => _transform.position;
    }
}