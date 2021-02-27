using DG.Tweening;
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

        public void SetPosition(Coordinate coordinate)
        {
            gameObject.name = $"[{coordinate}]";
            var colPosition = coordinate.Col * 1.0f;
            if (coordinate.Row % 2 == 1)
            {
                colPosition -= 0.5f;
            }

            SetPosition(new Vector2(colPosition, -coordinate.Row), false);
        }

        public StrikerView ConvertToStriker() => gameObject.AddComponent<StrikerView>();

        public Vector3 GetPosition() => _transform.position;
    }
}