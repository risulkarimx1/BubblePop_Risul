using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Async;

namespace Assets.Code.Bubble
{
    public class NumericMergeHelper
    {
        private bool IsValidNode(IBubbleNodeController n, IBubbleNodeController source, HashSet<IBubbleNodeController> visitedNodes)
        {
            return n != null && n.NodeValue == source.NodeValue && visitedNodes.Contains(n) == false;
        }

        public async UniTask<HashSet<IBubbleNodeController>> MergeNodes(IBubbleNodeController source)
        {
            var visitedNodes = BubbleUtility.Dfs(source, IsValidNode);

            var elements = visitedNodes.OrderByDescending(n => n.Position.y)
                .ThenBy(n => n.Position.x).ToArray();

            var nodesToRemove = new HashSet<IBubbleNodeController>();
            for (int i = 1; i < elements.Length; i++)
            {
                nodesToRemove.Add(elements[i]);
            }

            var willExplode = await PerformNumericMerge(elements);

            if (!willExplode) return nodesToRemove;

            var nodesToExplode = await GetExplodableNodes(elements[0]);
            foreach (var node in nodesToExplode)
            {
                if (nodesToRemove.Contains(node)) continue;

                nodesToRemove.Add(node);
            }

            return nodesToRemove;
        }

        private async UniTask<bool> PerformNumericMerge(IBubbleNodeController[] elements)
        {
            var willExplode = false;
            var index = elements.Length - 1;
            while (index > 0)
            {
                var current = elements[index];
                var upperNode = elements[index - 1];
                await UniTask.SwitchToMainThread();// brings to main thread
                await current.SetPositionAsync(upperNode.Position, true, 10,// leaves main thread
                    () =>
                    {
                        upperNode.NodeValue = upperNode.NodeValue * current.NodeValue;
                        if (upperNode.NodeValue > 2048)
                        {
                            upperNode.NodeValue = 2048;
                            willExplode = true;
                        }
                    });
                
                current.HideNode();
                index--;
            }
            
            // Coming back to main thread again
            await UniTask.SwitchToMainThread();
            return willExplode;
        }

        private async UniTask<List<IBubbleNodeController>> GetExplodableNodes(IBubbleNodeController source)
        {
            var nodesToRemove = new List<IBubbleNodeController>();
            var neighbors = source.GetNeighbors().Where(n => n != null);
            foreach (var bubbleNodeController in neighbors)
            {
                nodesToRemove.Add(bubbleNodeController);
                bubbleNodeController.ExplodeNode();
                await UniTask.Delay(10);
            }

            nodesToRemove.Add(source);
            await UniTask.Delay(10);

            return nodesToRemove;
        }
    }
}