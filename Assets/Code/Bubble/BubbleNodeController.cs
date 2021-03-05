using System.Collections.Generic;
using Assets.Code.Audio;
using DG.Tweening;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeController : IBubbleNodeController
    {
        private readonly BubbleNodeModel _bubbleNodeModel;
        private readonly BubbleNodeView _bubbleNodeView;
        private readonly ExplosionController.Factory _explosionFactory;
        private readonly CameraEffectsController _cameraEffectsController;
        private readonly AudioController _audioController;

        public IBubbleNodeController TopRight { get; set; }
        public IBubbleNodeController Right { get; set; }
        public IBubbleNodeController BottomRight { get; set; }
        public IBubbleNodeController BottomLeft { get; set; }
        public IBubbleNodeController Left { get; set; }
        public IBubbleNodeController TopLeft { get; set; }


        public int Id => _bubbleNodeView.gameObject.GetInstanceID();
        public BubbleType BubbleType => _bubbleNodeModel.BubbleType;
        public Vector2 Position => _bubbleNodeView.GetPosition();
        public StrikerView ConvertToStriker() => _bubbleNodeView.ConvertToStriker();


        public int NodeValue
        {
            get => _bubbleNodeModel.NodeValue.Value;
            set => _bubbleNodeModel.NodeValue.Value = value;
        }


        public BubbleNodeController(
            BubbleNodeModel bubbleNodeModel, 
            BubbleNodeView bubbleNodeView, 
            ExplosionController.Factory explosionFactory,
            CameraEffectsController cameraEffectsController,
            AudioController audioController)
        {
            _bubbleNodeModel = bubbleNodeModel;
            _bubbleNodeView = bubbleNodeView;
            _explosionFactory = explosionFactory;
            _cameraEffectsController = cameraEffectsController;
            _audioController = audioController;
            _bubbleNodeModel.NodeValue.Subscribe(val => { _bubbleNodeView.ValueText.text = val.ToString(); })
                .AddTo(_bubbleNodeView);
        }


        public void SetPosition(Vector2 position, bool animate = false, float speed = 1, TweenCallback callback = null, Ease ease = Ease.Linear)
        {
            _bubbleNodeView.SetPosition(position, animate, speed, callback, ease);
        }

        public async UniTask SetPositionAsync(Vector2 position, bool animate = false, float speed = 1, TweenCallback callback = null, Ease ease = Ease.Linear)
        {
            await _bubbleNodeView.SetPositionAsync(position, animate, speed, callback, ease);
        }
        
        public bool IsRemoved => _bubbleNodeView.isActiveAndEnabled == false;

        public override string ToString()
        {
            if (_bubbleNodeView == null) return string.Empty;
            return $"{_bubbleNodeView.name}";
        }

        public IBubbleNodeController[] GetNeighbors()
        {
            return new IBubbleNodeController[] {TopRight, Right, BottomRight, BottomLeft, Left, TopLeft};
        }

        public void SetNeighbor(int index, IBubbleNodeController node)
        {
            // Clock wise 
            if (index == 0) TopRight = node;
            if (index == 1) Right = node;
            if (index == 2) BottomRight = node;
            if (index == 3) BottomLeft = node;
            if (index == 4) Left = node;
            if (index == 5) TopLeft = node;
        }

        public void HideNode(TweenCallback callback = null, bool merge = false)
        {
            if (merge) _audioController.NumberMerge();
            _bubbleNodeView.AnimateHide(callback);
        }

        public async  UniTask ExplodeNodeAsync(TweenCallback callback = null)
        {
            _audioController.Explosion();
            ExplosionController explosion = null;
            DOTween.Sequence().AppendCallback(() =>
            {
                 explosion = _explosionFactory.Create(Position);
                 _cameraEffectsController.ShakeCamera();
            }).AppendCallback(() =>
            {
                explosion.Dispose();;
            });
            await UniTask.Delay(60);// duration of explosion animation: TODO Make it read from the animation
            HideNode(callback);
        }

        public void Remove()
        {
            if (IsRemoved) return;
            if (TopRight != null) TopRight.BottomLeft = null;
            if (Right != null) Right.Left = null;
            if (BottomRight != null) BottomRight.TopLeft = null;
            if (BottomLeft != null) BottomLeft.TopRight = null;
            if (Left != null) Left.Right = null;
            if (TopLeft != null) TopLeft.BottomRight = null;
            _bubbleNodeView.Remove();
        }

        public async UniTask DropNodeAsync(TweenCallback callback = null)
        {
            _bubbleNodeView.DropNode();
            var targetPosition = Position;
            targetPosition.x += targetPosition.x > 2 ? Random.Range(1, 2) : Random.Range(-1, -2);
            
            targetPosition.y = -12;
            await SetPositionAsync(targetPosition, true, 1f, callback, Ease.OutBounce);
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