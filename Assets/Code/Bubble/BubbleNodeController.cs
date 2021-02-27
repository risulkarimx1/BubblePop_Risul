using System.Collections.Generic;
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
        }

        public StrikerView ConvertToStriker() => _bubbleNodeView.ConvertToStriker();

        public void SetPosition(Vector2 position, bool animate = false, float speed = 1)
        {
            _bubbleNodeView.SetPosition(position, animate, speed);
        }

        public Vector2 Position => _bubbleNodeView.GetPosition();

        public int Id => _bubbleNodeView.gameObject.GetInstanceID();

        public override string ToString()
        {
            return $"{_bubbleNodeView.name}";
        }

        public void SetName(string name)
        {
            _bubbleNodeView.name = name;
        }

        public void ShowNeighbor()
        {
            _bubbleNodeView.Neighbors = new List<string>();
            _bubbleNodeView.Neighbors.Add($"TopRight: {TopRight}");
            _bubbleNodeView.Neighbors.Add($"Right: {Right}");
            _bubbleNodeView.Neighbors.Add($"Bottom Right: {BottomRight}");
            _bubbleNodeView.Neighbors.Add($"Bottom Left: {BottomLeft}");
            _bubbleNodeView.Neighbors.Add($"Left: {Left}");
            _bubbleNodeView.Neighbors.Add($"Top Left: {TopLeft}");
        }
    }
}