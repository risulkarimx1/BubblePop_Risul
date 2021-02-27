using UnityEngine;

namespace Assets.Code.Bubble
{
    public interface IBubbleNodeController
    {
        int Id { get; }
        IBubbleNodeController TopLeft { get; set; }
        IBubbleNodeController TopRight { get; set; }
        IBubbleNodeController Right { get; set; }
        IBubbleNodeController BottomRight { get; set; }
        IBubbleNodeController BottomLeft { get; set; }
        IBubbleNodeController Left { get; set; }
        StrikerView ConvertToStriker();
        void SetPosition(Vector2 position, bool animate = false, float speed = 1);
        Vector2 Position { get; }
        void SetName(string name);
        void ShowNeighbor();
        string ToString();
    }
}