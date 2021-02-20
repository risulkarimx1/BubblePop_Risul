namespace Assets.Code.Bubble
{
    public interface IBubbleNodeController
    {
        BubbleType BubbleType { get; set; }
        Coordinate Coordinate { get; set; }

        IBubbleNodeController TopLeft { get; set; }
        IBubbleNodeController TopRight { get; set; }
        IBubbleNodeController Right { get; set; }
        IBubbleNodeController BottomRight { get; set; }
        IBubbleNodeController BottomLeft { get; set; }
        IBubbleNodeController Left { get; set; }
    }
}