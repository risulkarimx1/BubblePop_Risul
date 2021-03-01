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

        public async UniTask<List<IBubbleNodeController>> RemoveIsolatedNodes(ConcurrentDictionary<int, IBubbleNodeController> viewToControllerMap)
        {
            var objectsToRemove = new List<IBubbleNodeController>();
            var visitedNodes = new Dictionary<IBubbleNodeController, bool>();

            foreach (var bubbleNodeController in viewToControllerMap)
            {
                visitedNodes.Add(bubbleNodeController.Value, false);
            }

            foreach (var node in viewToControllerMap)
            {
                if (visitedNodes[node.Value] == false)
                {
                    var connectedNodes = await GetConnectedNodes(node.Value, visitedNodes);
                    if (connectedNodes.Count > 0)
                    {
                        var connectedToCeiling = IsConnectedToCeiling(connectedNodes);
                        if (IsConnectedToCeiling(connectedNodes) == false)
                        {
                            objectsToRemove.AddRange(connectedNodes);
                        }
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

            return objectsToRemove;
        }

        private async UniTask<HashSet<IBubbleNodeController>> GetConnectedNodes(IBubbleNodeController sourceNode, Dictionary<IBubbleNodeController, bool> visitedNode)
        {
            var queue = new Queue<IBubbleNodeController>();
            visitedNode[sourceNode] = true;
            queue.Enqueue(sourceNode);

            var connectedNodes = new HashSet<IBubbleNodeController>();
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                var neighbors = currentNode.GetNeighbors().Where(n => n != null);
                if (connectedNodes.Contains(currentNode) == false) connectedNodes.Add(currentNode);
                foreach (var neighborNode in neighbors)
                {
                    if (visitedNode[neighborNode] == false)
                    {
                        queue.Enqueue(neighborNode);
                        visitedNode[neighborNode] = true;
                    }
                }
            }

            return connectedNodes;
        }

        private bool IsConnectedToCeiling(HashSet<IBubbleNodeController> connectedNodes)
        {
            var topNode = connectedNodes.Where(n => n.IsRemoved == false)
                .OrderByDescending(n => n.Position.y).FirstOrDefault();

            return topNode != null && (Math.Abs(topNode.Position.y - _ceilingY) < 0.001f);
        }
    }
}