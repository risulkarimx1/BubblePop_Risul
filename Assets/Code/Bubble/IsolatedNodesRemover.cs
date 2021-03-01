using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx.Async;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class IsolatedNodesRemover
    {
        private float _ceilingY = 0;

        public async UniTask RemoveIsolatedNodes(ConcurrentDictionary<int, IBubbleNodeController> viewToControllerMap)
        {
            var visitedNodes = new Dictionary<IBubbleNodeController, bool>();

            foreach (var bubbleNodeController in viewToControllerMap)
            {
                visitedNodes.Add(bubbleNodeController.Value, false);
            }

            foreach (var node in viewToControllerMap)
            {
                if (visitedNodes[node.Value] == false)
                {
                    var connectedNodes = await GetConnectedNodes(node.Value);
                    if (connectedNodes.Count > 0)
                    {
                        MarkConnectedNodesAsVisited(visitedNodes, connectedNodes);
                        var connectedToCeiling = IsConnectedToCeiling(connectedNodes);
                        var sb = new StringBuilder();
                        foreach (var connectedNode in connectedNodes)
                        {
                            sb.Append(connectedNode);
                            sb.Append(",");
                        }

                        Debug.Log($"Connected to Ceiling: {connectedToCeiling} for [{sb}]");
                    }
                }
            }
        }

        private async UniTask<HashSet<IBubbleNodeController>> GetConnectedNodes(IBubbleNodeController node)
        {
            var queue = new Queue<IBubbleNodeController>();
            var visitedNode = new HashSet<IBubbleNodeController>();
            if (visitedNode.Contains(node) == false) visitedNode.Add(node);
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                await UniTask.Delay(1000);
                var neighbors = currentNode.GetNeighbors().Where(n => n != null);
                foreach (var neighborNode in neighbors)
                {
                    if (visitedNode.Contains(neighborNode) == false)
                    {
                        queue.Enqueue(neighborNode);
                        visitedNode.Add(currentNode);
                    }
                }
            }

            return visitedNode;
        }

        private void MarkConnectedNodesAsVisited(Dictionary<IBubbleNodeController, bool> visitedNode,
            HashSet<IBubbleNodeController> connectedNodes)
        {
            foreach (var bubbleNodeController in connectedNodes)
            {
                visitedNode[bubbleNodeController] = true;
            }
        }

        private bool IsConnectedToCeiling(HashSet<IBubbleNodeController> connectedNodes)
        {
            var topNode = connectedNodes.Where(n => n.IsRemoved == false)
                .OrderByDescending(n => n.Position.y).FirstOrDefault();

            return topNode != null && (Math.Abs(topNode.Position.y - _ceilingY) < 0.001f);
        }
    }
}