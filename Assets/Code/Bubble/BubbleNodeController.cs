namespace Assets.Code.Bubble
{
    public class BubbleNodeController : IBubbleNodeController
    {
        private readonly BubbleNodeModel _bubbleNodeModel;
        private readonly BubbleNodeView _bubbleNodeView;
        private readonly Striker _striker;

        public BubbleNodeController(BubbleNodeModel bubbleNodeModel, BubbleNodeView bubbleNodeView, Striker striker = null)
        {
            _bubbleNodeModel = bubbleNodeModel;
            _bubbleNodeView = bubbleNodeView;
            
            if (striker == null)
            {
                Coordinate = _bubbleNodeModel.Coordinate;
            }
            else
            {
                _striker = striker;
                _bubbleNodeView.SetPosition(_striker.DefaultPosition);
            }
        }

        public void DestroyStriker()
        {
            if (_striker != null) _striker.DestroyComponent();
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