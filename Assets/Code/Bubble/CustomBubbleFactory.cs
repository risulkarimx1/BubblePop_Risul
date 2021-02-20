using System.Collections.Generic;
using Zenject;

namespace Assets.Code.Bubble
{
    public class CustomBubbleFactory: IFactory<BubbleType, Coordinate ,IBubbleNodeController>
    {
        private readonly DiContainer _container;
        private readonly BubblePrefabContainer _bubblePrefabContainer;

        public CustomBubbleFactory(DiContainer container, BubblePrefabContainer bubblePrefabContainer)
        {
            _container = container;
            _bubblePrefabContainer = bubblePrefabContainer;
        }

        public IBubbleNodeController Create(BubbleType bubbleType, Coordinate coordinate)
        {
            var bubblePrefab = _bubblePrefabContainer.GetBubbleOfType(bubbleType);
            var bubbleObject = _container.InstantiatePrefab(bubblePrefab);
            var nodeView = _container.InstantiateComponent<BubbleNodeView>(bubbleObject);
            var nodeModel = new BubbleNodeModel(bubbleType, coordinate);
            bubbleObject.name = $"{bubbleType}: [{coordinate}]";
            return _container.Instantiate<BubbleNodeController>(new List<object>() {nodeModel, nodeView});
        }
    }
}