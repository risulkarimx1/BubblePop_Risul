namespace Assets.Code.Bubble
{
    public class StrikerManager
    {
        private readonly BubbleFactory _bubbleFactory;

        public StrikerManager(BubbleFactory bubbleFactory)
        {
            _bubbleFactory = bubbleFactory;
            CreateStriker();
        }

        public void CreateStriker()
        {
            _bubbleFactory.Create(BubbleType.Blue, new Coordinate() {Col = -1, Row = -1});
        }
    }
}