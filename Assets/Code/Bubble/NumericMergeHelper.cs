using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class NumericMergeHelper
    {
        public async UniTask MergeNodes(IBubbleNodeController source)
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
            var willExplode = await MergeNodes(elements);

            if (willExplode)
            {
                await ExplodeNodes(elements[0]);
            }
        }

        private async UniTask<bool> MergeNodes(IBubbleNodeController[] elements)
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
            for (int i = elements.Length - 1; i > 0; i--)
            {
                elements[i].Remove();
            }

            return willExplode;
        }

        private async UniTask ExplodeNodes(IBubbleNodeController source)
        {
            var neighbors = source.GetNeighbors().Where(n => n != null);
            foreach (var bubbleNodeController in neighbors)
            {
                bubbleNodeController.HideNode();
                await UniTask.Delay(10);
            }

            source.HideNode();
        }
    }
}