using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.Code.LevelGeneration;
using Assets.Code.Managers;
using Assets.Code.Signals;
using UniRx.Async;
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
        private readonly NumericMergeHelper _numericMergeHelper;
        private readonly NodeIsolationHelper _nodeIsolationHelper;
        private readonly ColorMergeHelper _colorMergeHelper;
        private readonly GameStateController _gameStateController;

        private bool _isStrikerFinished = false;

        public BubbleGraph(BubbleFactory bubbleFactory,
            LevelDataContext levelDataContext,
            SignalBus signalBus,
            BubbleAttachmentHelper attachmentHelper,
            NumericMergeHelper numericMergeHelper,
            NodeIsolationHelper nodeIsolationHelper,
            ColorMergeHelper colorMergeHelper, GameStateController gameStateController)
        {
            _bubbleFactory = bubbleFactory;
            _levelDataContext = levelDataContext;
            _signalBus = signalBus;
            _attachmentHelper = attachmentHelper;
            _numericMergeHelper = numericMergeHelper;
            _nodeIsolationHelper = nodeIsolationHelper;
            _colorMergeHelper = colorMergeHelper;
            _gameStateController = gameStateController;
            _viewToControllerMap = new ConcurrentDictionary<int, IBubbleNodeController>();
            _attachmentHelper.Configure(_viewToControllerMap);
            _signalBus.Subscribe<BubbleCollisionSignal>(OnBubbleCollided);
            _signalBus.Subscribe<CeilingCollisionSignal>(OnCeilingCollision);
            _signalBus.Subscribe<StrikerFinishedSignal>(OnStrikerFinsihed);
        }

        private void OnStrikerFinsihed()
        {
            _isStrikerFinished = true;
        }

        public async UniTask InitializeAsync()
        {
            _gameStateController.CurrentSate = GameState.Loading;
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

            await RemapNeighborsAsync();
            _gameStateController.CurrentSate = GameState.WaitingToShoot;
        }

        private async UniTask RemapNeighborsAsync()
        {
            foreach (var bubbleNodeController in _viewToControllerMap)
            {
                bubbleNodeController.Value.ClearNeighbors();
                await _attachmentHelper.MapNeighbors(bubbleNodeController.Value);
                bubbleNodeController.Value.ShowNeighbor();
            }
        }

        private void OnBubbleCollided(BubbleCollisionSignal bubbleCollisionSignal)
        {
            _gameStateController.CurrentSate = GameState.Loading;
            var collision = bubbleCollisionSignal.CollisionObject;
            var colliderNodeController = _viewToControllerMap[collision.gameObject.GetInstanceID()];
            var strikerNodeController = bubbleCollisionSignal.StrikerNode;

            _viewToControllerMap.TryAdd(strikerNodeController.Id, strikerNodeController);
            _attachmentHelper.PlaceInGraph(collision, colliderNodeController, strikerNodeController, () =>
            {
                _ = PerformMergeOnNodeAsync(strikerNodeController);
            });
        }

        private async UniTask PerformMergeOnNodeAsync(IBubbleNodeController strikerNodeController)
        {
            await _attachmentHelper.MapNeighbors(strikerNodeController);

            var mergedNodeByNumber = await _numericMergeHelper.MergeNodes(strikerNodeController);
            RemoveNodes(mergedNodeByNumber);

            await RemapNeighborsAsync();

            var mergedNodeByColor = await _colorMergeHelper.MergeNodes(strikerNodeController);
            if (mergedNodeByColor != null) RemoveNodes(mergedNodeByColor);

            var isolatedNodes = _nodeIsolationHelper.GetIsolatedNodes(_viewToControllerMap);
            
            await DropAndRemoveNodes(isolatedNodes);
            await RemapNeighborsAsync();
            

            if (_viewToControllerMap.IsEmpty)
            {
                
                _gameStateController.CurrentSate = GameState.GameOverWin;
            }
            
            else if (_isStrikerFinished)
            {
                _gameStateController.CurrentSate = GameState.GameOverLose;
            }
            else
            {
                _gameStateController.CurrentSate = GameState.WaitingToShoot;
            }
        }

        private void OnCeilingCollision(CeilingCollisionSignal ceilingCollisionSignal)
        {
            _gameStateController.CurrentSate = GameState.Loading;
            var node = ceilingCollisionSignal.StrikerNode;
            var position = node.Position;
            position.y = 0;
            position.x = (float) Math.Round(position.x, 0);
            node.SetPosition(position, true, 10);
            AddNode(node);
            _ = PerformMergeOnNodeAsync(node);
        }

        private void RemoveNodes(IEnumerable<IBubbleNodeController> nodesToRemove)
        {
            foreach (var node in nodesToRemove)
            {
                _viewToControllerMap.TryRemove(node.Id, out IBubbleNodeController removedNode);
                removedNode?.Remove();
            }
        }

        private async UniTask DropAndRemoveNodes(IEnumerable<IBubbleNodeController> nodesToRemove)
        {
            var hasNodesToRemove = false;
            foreach (var node in nodesToRemove)
            {
                node.DropNode(() =>
                {
                    hasNodesToRemove = true;
                    _viewToControllerMap.TryRemove(node.Id, out IBubbleNodeController removedNode);
                    removedNode.ExplodeNode();
                    removedNode?.Remove();
                });
                await UniTask.Yield();
            }

            if (hasNodesToRemove)
            {
                GC.Collect();
                await UniTask.Delay(1000);
            }
        }

        public void AddNode(IBubbleNodeController bubbleController)
        {
            _viewToControllerMap.TryAdd(bubbleController.Id, bubbleController);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BubbleCollisionSignal>(OnBubbleCollided);
            _signalBus.Unsubscribe<CeilingCollisionSignal>(OnCeilingCollision);
            _signalBus.Unsubscribe<StrikerFinishedSignal>(OnStrikerFinsihed);
        }
    }
}