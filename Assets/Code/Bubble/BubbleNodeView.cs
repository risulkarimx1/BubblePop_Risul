using UnityEngine;

namespace Assets.Code.Bubble
{
    public class Striker : MonoBehaviour
    {
    }

    public class BubbleNodeView : MonoBehaviour
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        public void SetPosition(Coordinate coordinate)
        {
            var colPosition = coordinate.Col * 1.0f;
            if (coordinate.Row % 2 == 1)
            {
                colPosition -= 0.5f;
            }

            _transform.position = new Vector2(colPosition, -coordinate.Row);
        }
    }
}