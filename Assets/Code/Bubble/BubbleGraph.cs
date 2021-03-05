using System;
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
        private Dictionary<int, IBubbleNodeController> _viewToControllerMap;

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
            ColorMergeHelper colorMergeHelper, 
            GameStateController gameStateController)
        {
            _bubbleFactory = bubbleFactory;
            _levelDataContext = levelDataContext;
            _signalBus = signalBus;
            _attachmentHelper = attachmentHelper;
            _numericMergeHelper = numericMergeHelper;
            _nodeIsolationHelper = nodeIsolationHelper;
            _colorMergeHelper = colorMergeHelper;
            _gameStateController = gameStateController;
            _viewToControllerMap = new Dictionary<int, IBubbleNodeController>();
            _attachmentHelper.Configure(_viewToControllerMap);
            _signalBus.Subscribe<BubbleCollisionSignal>(OnBubbleCollided);
            _signalBus.Subscribe<CeilingCollisionSignal>(OnCeilingCollision);
            _signalBus.Subscribe<StrikerFinishedSignal>(OnStrikerFinished);
        }

        private void OnStrikerFinished()
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

            _viewToControllerMap.Add(strikerNodeController.Id, strikerNodeController);
            _attachmentHelper.PlaceInGraph(collision, colliderNodeController, strikerNodeController, () =>
            {
                _ = PerformMergeOnNodeAsync(strikerNodeController);
            });
        }

        private async UniTask PerformMergeOnNodeAsync(IBubbleNodeController strikerNodeController)
        {
            // map striker to its neighbor
            await _attachmentHelper.MapNeighbors(strikerNodeController);

            // Numeric merge
            var mergedNodeByNumber = await _numericMergeHelper.MergeNodesAsync(strikerNodeController);
            await RemoveNodesAsync(mergedNodeByNumber, "by number");

            // Map neighbor after numeric merge
            await RemapNeighborsAsync();

            // Merge by color and 3 nodes
            var mergedNodeByColor = await _colorMergeHelper.MergeNodes(strikerNodeController);
            if (mergedNodeByColor != null)
            {
                await RemoveNodesAsync(mergedNodeByColor, "by color");
                await RemapNeighborsAsync();
            }

            // Find and remove isolated nodes
            var isolatedNodes = _nodeIsolationHelper.GetIsolatedNodes(_viewToControllerMap);
            
            await DropAndRemoveNodes(isolatedNodes);
            await RemapNeighborsAsync();

            await CheckGameOverCondition();
        }

        private async UniTask CheckGameOverCondition()
        {
            await UniTask.DelayFrame(1);
            if (_viewToControllerMap.Count == 0)
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
            AddNode(node);
            node.SetPosition(position, true, 10, ()=>
            {
                _ = PerformMergeOnNodeAsync(node);
            });
        }

        private async UniTask RemoveNodesAsync(IEnumerable<IBubbleNodeController> nodesToRemove, string type)
        {
            foreach (var node in nodesToRemove)
            {
                await RemoveNodeAsync(node,type);
            }
        }

        private async UniTask DropAndRemoveNodes(IEnumerable<IBubbleNodeController> nodesToRemove)
        {
            var hasNodesToRemove = false;
            var dropTasks = new List<UniTask>();
            foreach (var node in nodesToRemove)
            {
                var t = node.DropNodeAsync(async () =>
                {   
                    await RemoveNodeAsync(node,"drop", true);
                    hasNodesToRemove = true;
                });
                dropTasks.Add(t);
            }

            await UniTask.WhenAll(dropTasks);
            
            if (hasNodesToRemove)
            {
                // make GC happy
                GC.Collect();
                await UniTask.Delay(1000);
            }
        }

        public void AddNode(IBubbleNodeController bubbleController)
        {
            _viewToControllerMap.Add(bubbleController.Id, bubbleController);
        }

        public async UniTask RemoveNodeAsync(IBubbleNodeController node,string type, bool explode = false)
        {
            _viewToControllerMap.Remove(node.Id);
            _signalBus.Fire(new ScoreUpdateSignal() { Score = node.NodeValue });
            if (explode)
            {
                await node.ExplodeNodeAsync();
            }
            node.Remove();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BubbleCollisionSignal>(OnBubbleCollided);
            _signalBus.Unsubscribe<CeilingCollisionSignal>(OnCeilingCollision);
            _signalBus.Unsubscribe<StrikerFinishedSignal>(OnStrikerFinished);
        }
    }
}