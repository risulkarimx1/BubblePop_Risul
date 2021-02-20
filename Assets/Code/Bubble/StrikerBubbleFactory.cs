using Zenject;

namespace Assets.Code.Bubble
{
    public class StrikerBubbleFactory : BubbleFactoryForInitialGraph
    {
        public StrikerBubbleFactory(DiContainer container, BubblePrefabContainer bubblePrefabContainer)
            : base(container, bubblePrefabContainer)
        {
        }

        public override IBubbleNodeController Create(BubbleType bubbleType, Coordinate coordinate)
        {
            var bubblePrefab = _bubblePrefabContainer.GetBubbleOfType(bubbleType);
            var bubbleObject = _container.InstantiatePrefab(bubblePrefab);
            var nodeView = _container.InstantiateComponent<BubbleNodeView>(bubbleObject);
            var strikerComponent = _container.InstantiateComponent<Striker>(bubbleObject);
            var nodeModel = new BubbleNodeModel(bubbleType, coordinate);
            bubbleObject.name = $"{bubbleType}: [{coordinate}]";
            return _container.Instantiate<BubbleNodeController>(new object[] {nodeModel, nodeView, strikerComponent});
        }
    }
}