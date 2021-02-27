using System.Collections.Generic;
using Assets.Code.Utils;
using UniRx.Async;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleAttachmentHelper
    {
        private Dictionary<int, IBubbleNodeController> _viewToControllerMap;

        public void Configure(Dictionary<int, IBubbleNodeController> viewToControllerMap)
        {
            _viewToControllerMap = viewToControllerMap;
        }
        
        public void PlaceInGraph(Collision2D collision, IBubbleNodeController collisionNodeController, IBubbleNodeController strikerNodeController)
        {
            var contactPoint = collision.contacts[0].point;
            contactPoint = collisionNodeController.Position - contactPoint;
            float angle = Mathf.Atan2(contactPoint.x, contactPoint.y) * 180 / Mathf.PI;
            if (angle < 0) angle = 360 + angle;
            var index = (int)(angle / 60);
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
    }
}