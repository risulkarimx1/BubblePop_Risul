using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeView : MonoBehaviour
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        public void SetPosition(Vector2 position) => _transform.position = position;
    }
}