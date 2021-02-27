namespace Assets.Code.Bubble
{
    public class BubbleNodeModel
    {
        public BubbleType BubbleType { get; set; }
        public BubbleNodeModel TopLeft { get; set; }
        public BubbleNodeModel TopRight { get; set; }
        public BubbleNodeModel Right { get; set; }
        public BubbleNodeModel BottomRight { get; set; }
        public BubbleNodeModel BottomLeft { get; set; }
        public BubbleNodeModel Left { get; set; }

        public BubbleNodeModel(BubbleType bubbleType)
        {
            BubbleType = bubbleType;
        }

        public override string ToString()
        {
            return $"BubbleType: {BubbleType}";
        }
    }
}