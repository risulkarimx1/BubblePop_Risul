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

        private IBubbleNodeController[] _neighbors;

        public BubbleNodeController(BubbleNodeModel bubbleNodeModel, BubbleNodeView bubbleNodeView)
        {
            _bubbleNodeModel = bubbleNodeModel;
            _bubbleNodeView = bubbleNodeView;
            Coordinate = bubbleNodeModel.Coordinate;
           
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
            if (index == 0)
            {
                Debug.Log($"is bottom left free {BottomLeft}");
            }
            if (index == 1)
            {
                Debug.Log($"is left free {Left}");
            }
            if (index == 2)
            {
                Debug.Log($"is top left free {TopLeft}");
            }
            if (index == 3)
            {
                Debug.Log($"is top right free {TopRight}");
            }
            if (index == 4)
            {
                Debug.Log($"is right free {Right}");
            }
            if (index == 5)
            {
                Debug.Log($"is bottom right free {BottomRight}");
            }
            _neighbors = new IBubbleNodeController[] {TopRight, Right, BottomRight, BottomLeft, Left, TopLeft};
            return index;
        }

        public StrikerView ConvertToStriker() => _bubbleNodeView.ConvertToStriker();

        public void SetPosition(Vector2 position, bool animate = false, float speed = 1)
        {
            _bubbleNodeView.SetPosition(position, animate, speed);
        }

        public Vector2 Position
        {
            get => _bubbleNodeView.GetPosition();
            set => _bubbleNodeView.SetPosition(value);
        }

        public int Id => _bubbleNodeView.gameObject.GetInstanceID();

        public override string ToString()
        {
            return $"{_bubbleNodeView.name}";
        }
    }
}