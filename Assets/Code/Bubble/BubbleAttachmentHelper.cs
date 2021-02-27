using UnityEngine;

namespace Assets.Code.Bubble
{
    public static class BubbleAttachmentHelper
    {
        // public static Vector2 GetDestination(Collision2D collision, IBubbleNodeController strikerNodeController, IBubbleNodeController collisionNodeController)
        // {
        //     var contactPoint = collision.contacts[0].point;
        //     contactPoint = collisionNodeController.Position - contactPoint;
        //
        //     float angle = Mathf.Atan2(contactPoint.x, contactPoint.y) * 180 / Mathf.PI;
        //     if (angle < 0) angle = 360 + angle;
        //     // angle = 360 - angle;
        //     // Debug.Log($"Collided with {collision.gameObject.name} at angle {angle}");
        //     // var attachmentDirection = GetDirectionFromAngle(angle, collisionNodeController);
        //     // return GetPositionFromDirection(attachmentDirection, collisionNodeController.Position);
        // }

        private static AttachmentDirection GetDirectionFromAngle(float angle, IBubbleNodeController collisionNodeController)
        {
            var direction = AttachmentDirection.None;

            if (angle.InBetween(0, 60))
                direction = AttachmentDirection.TopRight;
            else if (angle.InBetween(61, 120))
                direction = AttachmentDirection.Right;
            else if (angle.InBetween(121,180))
                direction = AttachmentDirection.BottomRight;
            else if (angle.InBetween(181, 240))
                direction = AttachmentDirection.BottomLeft;
            else if (angle.InBetween(241, 300))
                direction = AttachmentDirection.Left;
            else if (angle.InBetween(301, 360)) 
                direction = AttachmentDirection.TopLeft;

            // var nodeAtDirection = collisionNodeController.GetNodeAtDirection(direction);
            // if (nodeAtDirection == null) return direction;
            
            // on right side
            if (angle.InBetween(0, 180))
            {
                // check bottom right, right, topRight in order
                if (collisionNodeController.BottomRight == null) return AttachmentDirection.BottomRight;
                else if (collisionNodeController.Right == null) return AttachmentDirection.Right;
                else if (collisionNodeController.TopRight == null) return AttachmentDirection.TopRight;
            }
            // on left side
            else if (angle.InBetween(181, 360))
            {
                if (collisionNodeController.BottomLeft == null) return AttachmentDirection.BottomLeft;
                else if (collisionNodeController.Left == null) return AttachmentDirection.Left;
                else if (collisionNodeController.TopLeft == null) return AttachmentDirection.TopLeft;
            }
            
            // if nothing is free, check all and return first free slot
            // for (int i = 0; i < 6; i++)
            // {
            //     if (collisionNodeController[i] != null) return (AttachmentDirection) i;
            // }
            
            return direction;
        }

        public static Vector3 GetPositionFromDirection(AttachmentDirection direction, Vector3 attachmentNodePosition)
        {
            if (direction == AttachmentDirection.TopRight)
            {
                attachmentNodePosition.x++;
                attachmentNodePosition.y++;
                return attachmentNodePosition;
            }
            else if (direction == AttachmentDirection.Right)
            {
                attachmentNodePosition.x++;
                return attachmentNodePosition;
            }
            else if (direction == AttachmentDirection.BottomRight)
            {
                attachmentNodePosition.x++;
                attachmentNodePosition.y--;
                return attachmentNodePosition;
            }
            else if (direction == AttachmentDirection.BottomLeft)
            {
                attachmentNodePosition.x--;
                attachmentNodePosition.y--;
                return attachmentNodePosition;
            }
            else if (direction == AttachmentDirection.Left)
            {
                attachmentNodePosition.x--;
                return attachmentNodePosition;
            }
            else if (direction == AttachmentDirection.TopLeft)
            {
                attachmentNodePosition.x--;
                attachmentNodePosition.y++;
                return attachmentNodePosition;
            }

            return Vector3.zero;
        }

        
    }
}