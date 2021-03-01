using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Code.Utils;
using DG.Tweening;
using UniRx.Async;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleAttachmentHelper
    {
        private ConcurrentDictionary<int, IBubbleNodeController> _viewToControllerMap;

        public void Configure(ConcurrentDictionary<int, IBubbleNodeController> viewToControllerMap)
        {
            _viewToControllerMap = viewToControllerMap;
        }

        public void PlaceInGraph(Collision2D collision, IBubbleNodeController collisionNodeController,
            IBubbleNodeController strikerNodeController, TweenCallback callback)
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

            strikerNodeController.SetPosition(position, true,10, callback);
            
        }

        public async UniTask MapNeighbors(IBubbleNodeController strikerNodeController)
        {
            var topRightDirection = new Vector2(1, 1).normalized;
            var rightDirection = new Vector2(1, 0).normalized;
            var bottomRightDirection = new Vector2(1, -1).normalized;
            var bottomLeftDirection = new Vector2(-1, -1).normalized;
            var leftDirection = new Vector2(-1, 0).normalized;
            var topLeftDirection = new Vector2(-1, 1).normalized;

            if(strikerNodeController.IsRemoved) return;

            var tasks = new List<UniTask<IBubbleNodeController>>
            {
                MapNeighborAtDirection(strikerNodeController.Position, topRightDirection),
                MapNeighborAtDirection(strikerNodeController.Position, rightDirection),
                MapNeighborAtDirection(strikerNodeController.Position, bottomRightDirection),
                MapNeighborAtDirection(strikerNodeController.Position, bottomLeftDirection),
                MapNeighborAtDirection(strikerNodeController.Position, leftDirection),
                MapNeighborAtDirection(strikerNodeController.Position, topLeftDirection)
            };

            await UniTask.WhenAll(tasks);

            for (var i = 0; i < 6; i++)
            {
                strikerNodeController.SetNeighbor(i, tasks[i].Result);
            }
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