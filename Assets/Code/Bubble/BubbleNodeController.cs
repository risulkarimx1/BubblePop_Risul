using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeController : IBubbleNodeController
    {
        private readonly BubbleNodeModel _bubbleNodeModel;
        private readonly BubbleNodeView _bubbleNodeView;

        public IBubbleNodeController TopRight { get; set; }
        public IBubbleNodeController Right { get; set; }
        public IBubbleNodeController BottomRight { get; set; }
        public IBubbleNodeController BottomLeft { get; set; }
        public IBubbleNodeController Left { get; set; }
        public IBubbleNodeController TopLeft { get; set; }

        public BubbleNodeController(BubbleNodeModel bubbleNodeModel, BubbleNodeView bubbleNodeView)
        {
            _bubbleNodeModel = bubbleNodeModel;
            _bubbleNodeView = bubbleNodeView;
            _bubbleNodeModel.NodeValue.Subscribe(val =>
            {
                _bubbleNodeView.ValueText.text = val.ToString();
            }).AddTo(_bubbleNodeView);
        }

        public StrikerView ConvertToStriker() => _bubbleNodeView.ConvertToStriker();

        public void SetPosition(Vector2 position, bool animate = false, float speed = 1, TweenCallback callback = null)
        {
            _bubbleNodeView.SetPosition(position, animate, speed, callback);
        }

        public Vector2 Position => _bubbleNodeView.GetPosition();

        public int Id => _bubbleNodeView.gameObject.GetInstanceID();
        
        public bool IsRemoved => _bubbleNodeView.isActiveAndEnabled == false;

        public int NodeValue
        {
            get => _bubbleNodeModel.NodeValue.Value;
            set => _bubbleNodeModel.NodeValue.Value = value;
        }

        public override string ToString()
        {
            if (_bubbleNodeView == null) return string.Empty;
            return $"{_bubbleNodeView.name}";
        }

        public IBubbleNodeController[] GetNeighbors()
        {
            return new IBubbleNodeController[] { TopRight, Right, BottomRight, BottomLeft, Left, TopLeft };
        }

        public void SetNeighbor(int index, IBubbleNodeController node)
        {
            if (index == 0) TopRight = node;
            if (index == 1) Right = node;
            if (index == 2) BottomRight = node;
            if (index == 3) BottomLeft = node;
            if (index == 4) Left = node;
            if (index == 5) TopLeft = node;
        }

        public void HideNode(TweenCallback callback = null)
        {
            _bubbleNodeView.AnimateHide(callback);
        }

        public void Remove()
        {
            if(IsRemoved) return;
            if (TopRight != null) TopRight.BottomLeft = null;
            if (Right != null) Right.Left = null;
            if (BottomRight != null) BottomRight.TopLeft = null;
            if (BottomLeft != null) BottomLeft.TopRight = null;
            if (Left != null) Left.Right = null;
            if (TopLeft != null) TopLeft.BottomRight = null;
            _bubbleNodeView.Remove();
        }

        public void DropNode(TweenCallback callback = null)
        {
            var targetPosition = Position;
            targetPosition.y = -100;
            SetPosition(targetPosition,true, 0.1f, callback);
        }

        public void ClearNeighbors()
        {
            var neighbors = GetNeighbors();
            for (int i = 0; i < neighbors.Length; i++)
            {
                neighbors[i] = null;
            }
        }

        public void SetName(string name)
        {
            _bubbleNodeView.name = name;
        }

        public void ShowNeighbor()
        {
            _bubbleNodeView.Neighbors = new List<string>();
            _bubbleNodeView.Neighbors.Add($"TopRight: {TopRight}");
            _bubbleNodeView.Neighbors.Add($"Right: {Right}");
            _bubbleNodeView.Neighbors.Add($"Bottom Right: {BottomRight}");
            _bubbleNodeView.Neighbors.Add($"Bottom Left: {BottomLeft}");
            _bubbleNodeView.Neighbors.Add($"Left: {Left}");
            _bubbleNodeView.Neighbors.Add($"Top Left: {TopLeft}");
        }
    }
}