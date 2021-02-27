namespace Assets.Code.Bubble
{
    public class BubbleNodeModel
    {
        public BubbleType BubbleType { get; set; }
        
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