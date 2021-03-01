using System.Collections.Generic;
using System.Linq;
using UniRx.Async;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class ColorMergeHelper
    {
        public async UniTask<IBubbleNodeController[]> MergeNodes(IBubbleNodeController source)
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
                                && n.BubbleType == source.BubbleType
                                && visitedNodes.Contains(n) == false);
                foreach (var neighbor in neighbors)
                {
                    neighborsQue.Enqueue(neighbor);
                    await UniTask.Yield();
                }
            }

            if (visitedNodes.Count > 2)
            {
                IBubbleNodeController[] elements = visitedNodes.ToArray();
                await HideNodes(elements);
                return elements;
            }
            else
            {
                return null;
            }
            
        }

        public async UniTask HideNodes(IBubbleNodeController[] elements)
        {
            foreach (var bubbleNodeController in elements)
            {
                bubbleNodeController.HideNode();
                await UniTask.Delay(100);
            }
        }
    }
}