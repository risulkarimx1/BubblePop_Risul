namespace Assets.Code.Bubble
{
    public static class BubbleUtility
    {
        public static BubbleType ConvertColorToBubbleType(string color)
        {
            switch (color)
            {
                case "r":
                    return BubbleType.Red;
                case "g":
                    return BubbleType.Green;
                case "b":
                    return BubbleType.Blue;
                case "e":
                default:
                    return BubbleType.Empty;
            }
        }
    }
}