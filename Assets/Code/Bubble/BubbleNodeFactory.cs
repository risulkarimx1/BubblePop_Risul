using System;
using Zenject;

namespace Assets.Code.Bubble
{
    public class BubbleNodeFactory : IFactory<string, IBubbleNodeController>
    {
        protected readonly DiContainer _container;
        protected readonly BubbleDataContainer _bubbleDataContainer;

        public BubbleNodeFactory(DiContainer container, BubbleDataContainer bubbleDataContainer)
        {
            _container = container;
            _bubbleDataContainer = bubbleDataContainer;
        }

        public virtual IBubbleNodeController Create(string nodeInfo)
        {
            var info = nodeInfo.Split('-');
            var bubbleType = BubbleUtility.ConvertColorToBubbleType(info[0]);
            var value = Convert.ToInt32(info[1]);
            var bubblePrefab = _bubbleDataContainer.GetBubbleOfType(bubbleType);
            var bubbleView = _container.InstantiatePrefabForComponent<BubbleNodeView>(bubblePrefab);
            var nodeModel = new BubbleNodeModel(bubbleType,value);
            return _container.Instantiate<BubbleNodeController>(new object[] {nodeModel, bubbleView });
        }
    }
}