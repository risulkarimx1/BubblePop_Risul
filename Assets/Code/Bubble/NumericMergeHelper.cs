using System.Collections.Generic;
using System.Linq;
using Assets.Code.Utils;
using UniRx;
using UniRx.Async;

namespace Assets.Code.Bubble
{
    public class NumericMergeHelper
    {
        private bool IsValidNode(IBubbleNodeController n, IBubbleNodeController source,
            HashSet<IBubbleNodeController> visitedNodes)
        {
            return n != null && n.NodeValue == source.NodeValue && visitedNodes.Contains(n) == false;
        }

        public async UniTask<HashSet<IBubbleNodeController>> MergeNodesAsync(IBubbleNodeController source)
        {
            var visitedNodes = BubbleUtility.Dfs(source, IsValidNode);

            var elements = visitedNodes.OrderByDescending(n => n.Position.y)
                .ThenBy(n => n.Position.x).ToArray();

            var nodesToRemove = new HashSet<IBubbleNodeController>();
            for (int i = 1; i < elements.Length; i++)
            {
                nodesToRemove.Add(elements[i]);
            }

            var willExplode = await PerformNumericMergeAsync(elements);

            if (!willExplode) return nodesToRemove;

            var nodesToExplode = await GetExplodableNodes(elements[0]);
            foreach (var node in nodesToExplode)
            {
                if (nodesToRemove.Contains(node)) continue;

                nodesToRemove.Add(node);
            }

            return nodesToRemove;
        }

        private async UniTask<bool> PerformNumericMergeAsync(IBubbleNodeController[] elements)
        {
            var willExplode = false;
            var index = elements.Length - 1;
            while (index > 0)
            {
                var current = elements[index];
                var upperNode = elements[index - 1];
                await UniTask.SwitchToMainThread(); // brings to main thread
                await current.SetPositionAsync(upperNode.Position, true, 10, // leaves main thread
                    () =>
                    {
                        upperNode.NodeValue = upperNode.NodeValue * current.NodeValue;
                        if (upperNode.NodeValue > Constants.MaxBubbleValue)
                        {
                            upperNode.NodeValue = Constants.MaxBubbleValue;
                            willExplode = true;
                        }
                    });
                
                await UniTask.SwitchToMainThread();
                current.HideNode(null, true);
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
            nodesToRemove.Add(source);
            foreach (var bubbleNodeController in neighbors)
            {
                nodesToRemove.Add(bubbleNodeController);
                await bubbleNodeController.ExplodeNodeAsync();
            }

            return nodesToRemove;
        }
    }
}