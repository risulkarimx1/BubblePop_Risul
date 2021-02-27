using Zenject;

namespace Assets.Code.Bubble
{
    public class BubbleNodeFactory : IFactory<BubbleType, IBubbleNodeController>
    {
        protected readonly DiContainer _container;
        protected readonly BubbleDataContainer _bubbleDataContainer;

        public BubbleNodeFactory(DiContainer container, BubbleDataContainer bubbleDataContainer)
        {
            _container = container;
            _bubbleDataContainer = bubbleDataContainer;
        }

        public virtual IBubbleNodeController Create(BubbleType bubbleType)
        {
            var bubblePrefab = _bubbleDataContainer.GetBubbleOfType(bubbleType);
            var bubbleObject = _container.InstantiatePrefab(bubblePrefab);
            var nodeView = _container.InstantiateComponent<BubbleNodeView>(bubbleObject);
            var nodeModel = new BubbleNodeModel(bubbleType);
            return _container.Instantiate<BubbleNodeController>(new object[] {nodeModel, nodeView});
        }
    }
}