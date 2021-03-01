using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class NumericMergeHelper
    {
        public async UniTask<HashSet<IBubbleNodeController>> MergeNodes(IBubbleNodeController source)
        {
            var neighborsQue = new Queue<IBubbleNodeController>();
            neighborsQue.Enqueue(source);
            var visitedNodes = new HashSet<IBubbleNodeController>();
            while (neighborsQue.Count > 0)
            {
                var currentNode = neighborsQue.Dequeue();
                if (visitedNodes.Contains(currentNode) == false) visitedNodes.Add(currentNode);
                Debug.Log($"Dqd: {currentNode}");
                var neighbors = currentNode.GetNeighbors()
                    .Where(n => n != null
                                && n.NodeValue == source.NodeValue
                                && visitedNodes.Contains(n) == false);
                foreach (var neighbor in neighbors)
                {
                    neighborsQue.Enqueue(neighbor);
                    await UniTask.Yield();
                }
            }

            var elements = visitedNodes.OrderByDescending(n => n.Position.y)
                .ThenBy(n => n.Position.x).ToArray();

            var nodesToRemove = new HashSet<IBubbleNodeController>();
            for (int i = 1; i < elements.Length; i++)
            {
                nodesToRemove.Add(elements[i]);
            }

            var willExplode = await Merge(elements);

            if (willExplode)
            {
                var nodesToExplode = await ExplodeNodes(elements[0]);
                foreach (var node in nodesToExplode)
                {
                    if (nodesToRemove.Contains(node) == false)
                    {
                        nodesToRemove.Add(node);
                    }
                }
            }

            return nodesToRemove;
        }

        private async UniTask<bool> Merge(IBubbleNodeController[] elements)
        {
            var willExplode = false;
            var index = elements.Length - 1;
            while (index > 0)
            {
                var current = elements[index];
                var upperNode = elements[index - 1];
                current.SetPosition(upperNode.Position, true, 10,
                    () =>
                    {
                        upperNode.NodeValue = upperNode.NodeValue * current.NodeValue;
                        if (upperNode.NodeValue > 2048)
                        {
                            upperNode.NodeValue = 2048;
                            willExplode = true;
                        }
                    });
                await UniTask.Delay(100);

                current.HideNode();
                index--;
            }

            await UniTask.Delay(100);
            
            return willExplode;
        }

        private async UniTask<List<IBubbleNodeController>> ExplodeNodes(IBubbleNodeController source)
        {
            var nodesToRemove = new List<IBubbleNodeController>();
            var neighbors = source.GetNeighbors().Where(n => n != null);
            foreach (var bubbleNodeController in neighbors)
            {
                nodesToRemove.Add(bubbleNodeController);
                bubbleNodeController.HideNode();
                await UniTask.Delay(10);
            }

            nodesToRemove.Add(source);
            source.HideNode();
            await UniTask.Delay(30);

            return nodesToRemove;
        }
    }
}