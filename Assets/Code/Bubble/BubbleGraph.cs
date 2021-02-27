using System;
using System.Collections.Generic;
using Assets.Code.LevelGeneration;
using Assets.Code.Utils;
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

        public BubbleGraph(BubbleFactory bubbleFactory, LevelDataContext levelDataContext, SignalBus signalBus)
        {
            _bubbleFactory = bubbleFactory;
            _levelDataContext = levelDataContext;
            _signalBus = signalBus;
            _viewToControllerMap = new Dictionary<int, IBubbleNodeController>();
            _signalBus.Subscribe<BubbleCollisionSignal>(OnBubbleCollided);
        }

        public async UniTask InitializeBubbleGraph()
        {
            string levelData = _levelDataContext.GetSelectedLevelData();
            var lines = levelData.Split('\n');
            Vector2 pos = Vector2.zero;
            var nodeCounter = 0;
            for (var row = 0; row < lines.Length; row++)
            {
                var text = lines[row].Trim();

                var colors = text.Split(',');
                pos.x = 0;
                if (row % 2 == 1) pos.x -= 0.5f;
                foreach (var color in colors)
                {
                    var bubbleType = BubbleUtility.ConvertColorToBubbleType(color);

                    if (bubbleType == BubbleType.Empty)
                    {
                        pos.x++;
                        continue;
                    }

                    var node = _bubbleFactory.Create(bubbleType);
                    node.SetName($"Node : {nodeCounter++}");
                    node.SetPosition(pos);
                    pos.x++;
                    AddNode(node);
                }

                pos.y--;
            }

            foreach (var bubbleNodeController in _viewToControllerMap)
            {
                await MapNeighbors(bubbleNodeController.Value);
                bubbleNodeController.Value.ShowNeighbor();
            }

        }

        public async UniTask MapNeighbors(IBubbleNodeController strikerNodeController)
        {
            var topRightDirection = new Vector2(1, 1).normalized;
            var rightDirection = new Vector2(1, 0).normalized;
            var bottomRightDirection = new Vector2(1, -1).normalized;
            var bottomLeftDirection = new Vector2(-1, -1).normalized;
            var leftDirection = new Vector2(-1, 0).normalized;
            var topLeftDirection = new Vector2(-1, 1).normalized;

            strikerNodeController.TopRight = await MapNeighborAtDirection(strikerNodeController.Position, topRightDirection);
            strikerNodeController.Right = await MapNeighborAtDirection(strikerNodeController.Position, rightDirection);
            strikerNodeController.BottomRight = await MapNeighborAtDirection(strikerNodeController.Position, bottomRightDirection);

            strikerNodeController.BottomLeft = await MapNeighborAtDirection(strikerNodeController.Position, bottomLeftDirection);
            strikerNodeController.Left = await MapNeighborAtDirection(strikerNodeController.Position, leftDirection);
            strikerNodeController.TopLeft = await MapNeighborAtDirection(strikerNodeController.Position, topLeftDirection);
           
        }

        public async UniTask<IBubbleNodeController> MapNeighborAtDirection(Vector2 origin, Vector2 direction)
        {
            var hit = Physics2D.Raycast(origin, direction, 1, Constants.BubbleLayerMask);
            await UniTask.Yield();
            Debug.DrawRay(origin, direction, Color.cyan);
            if (hit.collider != null)
            {
                Debug.DrawRay(origin, direction, Color.red);
                return _viewToControllerMap[hit.collider.gameObject.GetInstanceID()];
            }
            return null;
        }

        private void OnBubbleCollided(BubbleCollisionSignal bubbleCollisionSignal)
        {
            var collision = bubbleCollisionSignal.CollisionObject;
            var colliderNodeController = _viewToControllerMap[collision.gameObject.GetInstanceID()];
            var strikerNodeController = bubbleCollisionSignal.StrikerNode;
            
            _viewToControllerMap.Add(strikerNodeController.Id, strikerNodeController);
            RepositionBubble(collision, colliderNodeController, strikerNodeController);
            _ = MapNeighbors(strikerNodeController);
            strikerNodeController.ShowNeighbor();
        }
        
        public void RepositionBubble(Collision2D collision, IBubbleNodeController collisionNodeController, IBubbleNodeController strikerNodeController)
        {
            var contactPoint = collision.contacts[0].point;
            contactPoint = collisionNodeController.Position - contactPoint;
            float angle = Mathf.Atan2(contactPoint.x, contactPoint.y) * 180 / Mathf.PI;
            if (angle < 0) angle = 360 + angle;
            var index = (int) (angle / 60);
            var position = collisionNodeController.Position;
            if (index == 0)
            {
                // bottom left of other
                position.x -= 0.5f;
                position.y--;
            }
            else if (index == 1)
            {
                // left of other
                position.x--;
            }
            else if (index == 2)
            {
                // top left of other
                position.x -= 0.5f;
                position.y++;
            }
            else if (index == 3)
            {
                // top right of other
                position.x += 0.5f;
                position.y++;
            }
            else if (index == 4)
            {
                // right of other
                position.x++;
            }
            else if (index == 5)
            {
                // bottom right of other
                position.x += 0.5f;
                position.y--;
            }

            strikerNodeController.SetPosition(position);
        }

        public void AddNode(IBubbleNodeController bubbleController)
        {
            _viewToControllerMap.Add(bubbleController.Id, bubbleController);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BubbleCollisionSignal>(OnBubbleCollided);
        }
    }
}