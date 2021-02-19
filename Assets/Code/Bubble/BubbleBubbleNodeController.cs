using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleBubbleNodeController : IBubbleNodeController
    {
        private readonly BubbleNodeModel _bubbleNodeModel;
        private readonly BubbleNodeView _bubbleNodeView;

        public BubbleBubbleNodeController(BubbleNodeModel bubbleNodeModel, BubbleNodeView bubbleNodeView)
        {
            _bubbleNodeModel = bubbleNodeModel;
            _bubbleNodeView = bubbleNodeView;
        }

        public void SetPosition(Coordinate coordinate)
        {
            var colPosition = coordinate.Col * 1.0f;
            if (coordinate.Row % 2 == 1)
            {
                colPosition -= 0.5f;
            }

            _bubbleNodeView.SetPosition(new Vector2(colPosition, -coordinate.Row));
        }
    }
}