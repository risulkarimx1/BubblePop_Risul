using DG.Tweening;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public interface IBubbleNodeController
    {
        int Id { get; }
        bool IsRemoved { get; }
        int NodeValue { get; set; }
        BubbleType BubbleType { get;}
        IBubbleNodeController TopLeft { get; set; }
        IBubbleNodeController TopRight { get; set; }
        IBubbleNodeController Right { get; set; }
        IBubbleNodeController BottomRight { get; set; }
        IBubbleNodeController BottomLeft { get; set; }
        IBubbleNodeController Left { get; set; }
        StrikerView ConvertToStriker();
        void SetPosition(Vector2 position, bool animate = false, float speed = 1, TweenCallback callback = null);
        Vector2 Position { get; }
        void SetName(string name);
        void ShowNeighbor();
        IBubbleNodeController[] GetNeighbors();
        void HideNode(TweenCallback callback = null);
        void Remove();
        void DropNode(TweenCallback callback = null);
        void SetNeighbor(int index, IBubbleNodeController node);
        void ClearNeighbors();
    }
}