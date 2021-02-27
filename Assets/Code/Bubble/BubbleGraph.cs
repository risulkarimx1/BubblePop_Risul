using System;
using System.Collections.Generic;
using Assets.Code.LevelGeneration;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Bubble
{
    public class BubbleGraph : IDisposable
    {
        private readonly BubbleFactory _bubbleFactory;
        private readonly LevelDataContext _levelDataContext;
        private readonly SignalBus _signalBus;
        private Dictionary<Coordinate, IBubbleNodeController> _bubbles;
        private Dictionary<int, IBubbleNodeController> _viewToControllerMap;

        public BubbleGraph(BubbleFactory bubbleFactory, LevelDataContext levelDataContext, SignalBus signalBus)
        {
            _bubbleFactory = bubbleFactory;
            _levelDataContext = levelDataContext;
            _signalBus = signalBus;
            _bubbles = new Dictionary<Coordinate, IBubbleNodeController>();
            _viewToControllerMap = new Dictionary<int, IBubbleNodeController>();
            _signalBus.Subscribe<BubbleCollisionSignal>(OnBubbleCollided);
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
            strikerNodeController.BottomRight= MapNeighborAtDirection(strikerNodeController.Position, bottomRightDirection);

            strikerNodeController.BottomLeft = MapNeighborAtDirection(strikerNodeController.Position, bottomLeftDirection);
            strikerNodeController.Left = MapNeighborAtDirection(strikerNodeController.Position, leftDirection);
            strikerNodeController.TopLeft = MapNeighborAtDirection(strikerNodeController.Position, topLeftDirection);
        }

        public IBubbleNodeController MapNeighborAtDirection(Vector2 origin, Vector2 direction)
        {
            var hit = Physics2D.Raycast(origin, direction, 1, Constants.BubbleLayerMask);
            if (hit.collider != null)
            {
                // Debug.Log($"hit collider game object name {hit.collider.gameObject.name} to direction");
                return _viewToControllerMap[hit.collider.gameObject.GetInstanceID()];
            }

            return null;
        }

        public void RepositionBubble(Collision2D collision, IBubbleNodeController collisionNodeController, IBubbleNodeController strikerNodeController)
        {
            var contactPoint = collision.contacts[0].point;
            contactPoint = collisionNodeController.Position - contactPoint;
            float angle = Mathf.Atan2(contactPoint.x, contactPoint.y) * 180 / Mathf.PI;
            if (angle < 0) angle = 360 + angle;
            var index = (int) (angle / 60);
            Debug.Log($"Collided with {collisionNodeController} at angle {(int)(angle / 60)}");
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
                coordinate.Col++;
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
                coordinate.Col--;
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

        public void InitializeBubbleGraph()
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

                    var node = _bubbleFactory.Create(bubbleType, new Coordinate() {Row = row, Col = col});
                    AddNode(node);
                    if (col > 0)
                    {
                        var leftIndex = col - 1;
                        _bubbles.TryGetValue(new Coordinate() {Row = row, Col = leftIndex}, out var leftNode);
                        if (leftNode != null)
                        {
                            node.Left = _bubbles[new Coordinate() {Row = row, Col = leftIndex}];
                            node.Left.Right = node;
                        }
                    }

                    if (row > 0)
                    {
                        if (col > 0)
                        {
                            _bubbles.TryGetValue(new Coordinate {Col = col - 1, Row = row}, out var upperLeftNode);
                            if (upperLeftNode != null)
                            {
                                node.TopLeft = upperLeftNode;
                                upperLeftNode.BottomRight = node;
                            }
                        }

                        _bubbles.TryGetValue(new Coordinate() {Col = col, Row = row - 1}, out var upperNode);

                        if (upperNode != null)
                        {
                            node.TopRight = upperNode;
                            upperNode.BottomLeft = node;
                        }
                    }
                }
            }
        }

        public void AddNode(IBubbleNodeController bubbleController)
        {
            _bubbles.Add(bubbleController.Coordinate, bubbleController);
            _viewToControllerMap.Add(bubbleController.Id, bubbleController);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BubbleCollisionSignal>(OnBubbleCollided);
        }
    }
}