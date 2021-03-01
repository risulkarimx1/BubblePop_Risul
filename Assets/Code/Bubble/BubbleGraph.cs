using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.Code.LevelGeneration;
using UniRx.Async;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Assets.Code.Bubble
{
    public class BubbleGraph : IDisposable
    {
        private readonly BubbleFactory _bubbleFactory;
        private readonly LevelDataContext _levelDataContext;
        private readonly SignalBus _signalBus;
        private ConcurrentDictionary<int, IBubbleNodeController> _viewToControllerMap;

        private BubbleAttachmentHelper _attachmentHelper;
        private readonly NumericMergeHelper _numericMerge;
        private readonly IsolatedNodesRemover _isolatedNodesRemover;

        public BubbleGraph(BubbleFactory bubbleFactory, LevelDataContext levelDataContext, SignalBus signalBus,
            BubbleAttachmentHelper attachmentHelper, NumericMergeHelper numericMerge, IsolatedNodesRemover isolatedNodesRemover)
        {
            _bubbleFactory = bubbleFactory;
            _levelDataContext = levelDataContext;
            _signalBus = signalBus;
            _attachmentHelper = attachmentHelper;
            _numericMerge = numericMerge;
            _isolatedNodesRemover = isolatedNodesRemover;
            _viewToControllerMap = new ConcurrentDictionary<int, IBubbleNodeController>();
            _attachmentHelper.Configure(_viewToControllerMap);
            _signalBus.Subscribe<BubbleCollisionSignal>(OnBubbleCollided);
        }

        public async UniTask Initialize()
        {
            string levelData = _levelDataContext.GetSelectedLevelData();
            var lines = levelData.Split('\n');
            Vector2 pos = Vector2.zero;
            var nodeCounter = 0;
            for (var row = 0; row < lines.Length; row++)
            {
                var text = lines[row].Trim();

                var columns = text.Split(',');
                pos.x = 0;
                if (row % 2 == 1) pos.x -= 0.5f;
                foreach (var column in columns)
                {
                    var color = column.Split('-')[0];
                    var bubbleType = BubbleUtility.ConvertColorToBubbleType(color);

                    if (bubbleType == BubbleType.Empty)
                    {
                        pos.x++;
                        continue;
                    }

                    var node = _bubbleFactory.Create(column);
                    node.SetName($"Node : {nodeCounter++}");
                    node.SetPosition(pos);
                    pos.x++;
                    AddNode(node);
                }

                pos.y--;
            }

            await RemapNeighbors();
        }

        private async UniTask RemapNeighbors()
        {
            foreach (var bubbleNodeController in _viewToControllerMap)
            {
                if (bubbleNodeController.Value.IsRemoved == false)
                {
                    bubbleNodeController.Value.ClearNeighbors();
                    await _attachmentHelper.MapNeighbors(bubbleNodeController.Value);
                    bubbleNodeController.Value.ShowNeighbor();
                }

            }
        }
        
        private void OnBubbleCollided(BubbleCollisionSignal bubbleCollisionSignal)
        {
            var collision = bubbleCollisionSignal.CollisionObject;
            var colliderNodeController = _viewToControllerMap[collision.gameObject.GetInstanceID()];
            var strikerNodeController = bubbleCollisionSignal.StrikerNode;

            _viewToControllerMap.TryAdd(strikerNodeController.Id, strikerNodeController);
            _attachmentHelper.PlaceInGraph(collision, colliderNodeController, strikerNodeController, () =>
            {
                _ = MapNeighbors(strikerNodeController);
            });
        }

        private async UniTask MapNeighbors(IBubbleNodeController strikerNodeController)
        {
            await _attachmentHelper.MapNeighbors(strikerNodeController);
            var nodesToRemove = await _numericMerge.MergeNodes(strikerNodeController);
            RemoveNodes(nodesToRemove);
            // foreach (var node in nodesToRemove)
            // {
            //     _viewToControllerMap.TryRemove(node.Id, out IBubbleNodeController removedNode);
            //     Debug.Log($"Removed node: {removedNode}");
            //     removedNode.Remove();
            // }
            await RemapNeighbors();
            var isolatedNodes = await _isolatedNodesRemover.RemoveIsolatedNodes(_viewToControllerMap);
            RemoveNodes(isolatedNodes);
            await RemapNeighbors();
            // strikerNodeController.ShowNeighbor();
        }

        private void RemoveNodes(IEnumerable<IBubbleNodeController> nodesToRemove)
        {
            foreach (var node in nodesToRemove)
            {
                _viewToControllerMap.TryRemove(node.Id, out IBubbleNodeController removedNode);
                Debug.Log($"Removed node: {removedNode}");
                removedNode.Remove();
            }
        }
        
        public void AddNode(IBubbleNodeController bubbleController)
        {
            _viewToControllerMap.TryAdd(bubbleController.Id, bubbleController);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BubbleCollisionSignal>(OnBubbleCollided);
        }
    }
}