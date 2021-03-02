using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.Bubble
{
    public static class BubbleUtility
    {
        public static BubbleType ConvertColorToBubbleType(string color)
        {
            switch (color)
            {
                case "r":
                    return BubbleType.Red;
                case "g":
                    return BubbleType.Green;
                case "b":
                    return BubbleType.Blue;
                case "e":
                default:
                    return BubbleType.Empty;
            }
        }

        public static IEnumerable<IBubbleNodeController> Dfs(IBubbleNodeController source,
            Func<IBubbleNodeController, IBubbleNodeController, HashSet<IBubbleNodeController>, bool> isValid)
        {
            int count = 0;
            return Dfs(source, isValid, ref count);
        }

        public static IEnumerable<IBubbleNodeController> Dfs(IBubbleNodeController source,
            Func<IBubbleNodeController, IBubbleNodeController, HashSet<IBubbleNodeController>, bool> isValid, ref int count)
        {
            var neighborsQue = new Queue<IBubbleNodeController>();
            neighborsQue.Enqueue(source);
            var visitedNodes = new HashSet<IBubbleNodeController>();
            while (neighborsQue.Count > 0)
            {
                var currentNode = neighborsQue.Dequeue();
                if (visitedNodes.Contains(currentNode) == false) visitedNodes.Add(currentNode);
                var neighbors = currentNode.GetNeighbors()
                    .Where(n => isValid(n, source, visitedNodes));
                foreach (var neighbor in neighbors)
                {
                    neighborsQue.Enqueue(neighbor);
                }
            }

            count = visitedNodes.Count;
            return visitedNodes;
        }
    }
}