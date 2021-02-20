using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeController : IBubbleNodeController
    {
        private readonly BubbleNodeModel _bubbleNodeModel;
        private readonly BubbleNodeView _bubbleNodeView;

        public BubbleNodeController(BubbleNodeModel bubbleNodeModel, BubbleNodeView bubbleNodeView)
        {
            _bubbleNodeModel = bubbleNodeModel;
            _bubbleNodeView = bubbleNodeView;
            Coordinate = _bubbleNodeModel.Coordinate;
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

        public BubbleType BubbleType
        {
            get => _bubbleNodeModel.BubbleType;
            set => _bubbleNodeModel.BubbleType = value;
        }

        public IBubbleNodeController TopLeft { get; set; }
        public IBubbleNodeController TopRight { get; set; }
        public IBubbleNodeController Right { get; set; }
        public IBubbleNodeController BottomRight { get; set; }
        public IBubbleNodeController BottomLeft { get; set; }
        public IBubbleNodeController Left { get; set; }
    }
}