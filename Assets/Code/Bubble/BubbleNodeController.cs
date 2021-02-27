using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeController : IBubbleNodeController
    {
        private readonly BubbleNodeModel _bubbleNodeModel;
        private readonly BubbleNodeView _bubbleNodeView;

        public IBubbleNodeController TopRight { get; set; }
        public IBubbleNodeController Right { get; set; }
        public IBubbleNodeController BottomRight { get; set; }
        public IBubbleNodeController BottomLeft { get; set; }
        public IBubbleNodeController Left { get; set; }
        public IBubbleNodeController TopLeft { get; set; }

        private readonly IBubbleNodeController[] _neighbors;

        public BubbleNodeController(BubbleNodeModel bubbleNodeModel, BubbleNodeView bubbleNodeView)
        {
            _bubbleNodeModel = bubbleNodeModel;
            _bubbleNodeView = bubbleNodeView;
            Coordinate = bubbleNodeModel.Coordinate;
            _neighbors = new IBubbleNodeController[] {TopRight, Right, BottomRight, BottomLeft, Left, TopLeft};
        }

        public Coordinate Coordinate
        {
            get => _bubbleNodeModel.Coordinate;
            set
            {
                _bubbleNodeModel.Coordinate = value;
                _bubbleNodeView.SetPosition(_bubbleNodeModel.Coordinate);
            }
        }

        public int GetFreeNeighbor(int index)
        {
            var left = index;
            var right = index + 1;
            while (left >= 0)
            {
                if (_neighbors[left] == null) return left;
                left--;
            }

            while (right < _neighbors.Length)
            {
                if (_neighbors[right] == null) return right;
                right++;
            }

            return -1;
        }

        public StrikerView ConvertToStriker() => _bubbleNodeView.ConvertToStriker();

        public void SetPosition(Vector2 position, bool animate = false, float speed = 1) => _bubbleNodeView.SetPosition(position, animate, speed);

        public Vector2 Position
        {
            get => _bubbleNodeView.GetPosition();
            set => _bubbleNodeView.SetPosition(value);
        }

        public int Id => _bubbleNodeView.gameObject.GetInstanceID();

        public override string ToString() => $"{_bubbleNodeView.name}";
    }
}