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
            for (var row = 0; row < lines.Length; row++)
            {
                var text = lines[row].Trim();

                var colors = text.Split(',');

                for (var col = 0; col < colors.Length; col++)
                {
                    var color = colors[col];
                    var bubbleType = BubbleUtility.ConvertColorToBubbleType(color);

                    if (bubbleType == BubbleType.Empty) continue;

                    var node = _bubbleFactory.Create(bubbleType, new Coordinate() { Row = row, Col = col });
                    AddNode(node);
                }
            }

            foreach (var bubbleNodeController in _viewToControllerMap)
            {
                MapNeighbors(bubbleNodeController.Value);
                await UniTask.Yield();
            }

        }

        public void MapNeighbors(IBubbleNodeController strikerNodeController)
        {
            var topRightDirection = new Vector2(1, 1).normalized;
            var rightDirection = new Vector2(1, 0).normalized;
            var bottomRightDirection = new Vector2(1, -1).normalized;
            var bottomLeftDirection = new Vector2(-1, -1).normalized;
            var leftDirection = new Vector2(-1, 0).normalized;
            var topLeftDirection = new Vector2(-1, 1).normalized;

            strikerNodeController.TopRight = MapNeighborAtDirection(strikerNodeController.Position, topRightDirection);
            strikerNodeController.Right = MapNeighborAtDirection(strikerNodeController.Position, rightDirection);
            strikerNodeController.BottomRight = MapNeighborAtDirection(strikerNodeController.Position, bottomRightDirection);

            strikerNodeController.BottomLeft = MapNeighborAtDirection(strikerNodeController.Position, bottomLeftDirection);
            strikerNodeController.Left = MapNeighborAtDirection(strikerNodeController.Position, leftDirection);
            strikerNodeController.TopLeft = MapNeighborAtDirection(strikerNodeController.Position, topLeftDirection);
           
        }

        public IBubbleNodeController MapNeighborAtDirection(Vector2 origin, Vector2 direction)
        {
            var hit = Physics2D.Raycast(origin, direction, 1, Constants.BubbleLayerMask);
            Debug.DrawRay(origin, direction, Color.cyan);
            if (hit.collider != null)
            {
                Debug.DrawRay(origin, direction, Color.red);
                Debug.Log($"From {origin} - hit collider game object name {hit.collider.gameObject.name} to direction");
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
            MapNeighbors(strikerNodeController);
        }

        

        public void RepositionBubble(Collision2D collision, IBubbleNodeController collisionNodeController, IBubbleNodeController strikerNodeController)
        {
            var contactPoint = collision.contacts[0].point;
            contactPoint = collisionNodeController.Position - contactPoint;
            float angle = Mathf.Atan2(contactPoint.x, contactPoint.y) * 180 / Mathf.PI;
            if (angle < 0) angle = 360 + angle;
            var index = (int) (angle / 60);
            Debug.Log($"[{strikerNodeController}] - Collided with {collisionNodeController} at angle {(int)(angle / 60)}");
            index = collisionNodeController.GetFreeNeighbor(index);
            Debug.Log($"Now Index {index}");
            var position = collisionNodeController.Position;
            var coordinate = collisionNodeController.Coordinate;
            if (index == 0)
            {
                // bottom left of other
                position.x -= 0.5f;
                position.y--;
                coordinate.Row++;
            }
            else if (index == 1)
            {
                // left of other
                position.x--;
                coordinate.Col--;
            }
            else if (index == 2)
            {
                // top left of other
                position.x -= 0.5f;
                position.y++;
                coordinate.Row--;
            }
            else if (index == 3)
            {
                // top right of other
                position.x += 0.5f;
                position.y++;
                coordinate.Col++;
                coordinate.Row--;
            }
            else if (index == 4)
            {
                // right of other
                position.x++;
                coordinate.Col++;
            }
            else if (index == 5)
            {
                // bottom right of other
                position.x += 0.5f;
                position.y--;
                coordinate.Row++;
                coordinate.Col++;
            }

            strikerNodeController.Position = position;
           // strikerNodeController.Coordinate = coordinate;
        }

        public void AddNode(IBubbleNodeController bubbleController)
        {
            // _bubbles.Add(bubbleController.Coordinate, bubbleController);
            _viewToControllerMap.Add(bubbleController.Id, bubbleController);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BubbleCollisionSignal>(OnBubbleCollided);
        }
    }
}