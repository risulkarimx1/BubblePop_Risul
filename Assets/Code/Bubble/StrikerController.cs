using System;
using Assets.Code.Signals;
using Assets.Code.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Code.Bubble
{
    public class StrikerController
    {
        private readonly StrikerView _strikerView;
        private readonly IBubbleNodeController _bubbleNodeController;
        private readonly IDisposable _collisionEnterDisposable;

        public StrikerController(
            BubbleFactory bubbleNodeFactory,
            BubbleDataContainer bubbleDataContainer,
            Vector2 position, string nodeInfo,
            SignalBus signalBus,
            CameraEffectsController cameraEffectsController)
        {
            _bubbleNodeController = bubbleNodeFactory.Create(nodeInfo);
            _strikerView = _bubbleNodeController.ConvertToStriker();

            _strikerView.Configure(bubbleDataContainer.StrikerPhysicsMaterial, position);
            _collisionEnterDisposable = _strikerView.CollisionEnter2D.Subscribe(other =>
            {
                if (other.collider.CompareTag(Constants.BubbleTag))
                {
                    _strikerView.ResetCollider();
                    signalBus.Fire(new BubbleCollisionSignal()
                    {
                        CollisionObject = other,
                        StrikerNode = _bubbleNodeController,
                    });
                    cameraEffectsController.ShowRipple(_bubbleNodeController.Position);
                    DestroyComponent();
                }
                else if (other.collider.CompareTag(Constants.CeilingTag))
                {
                    _strikerView.ResetCollider();
                    signalBus.Fire(new CeilingCollisionSignal()
                    {
                        StrikerNode = _bubbleNodeController
                    });

                    DestroyComponent();
                }
            }).AddTo(_strikerView);
        }

        public void SetName(string name) => _strikerView.SetName(name);

        public void Strike(Vector2 direction) => _strikerView.Strike(direction);

        public void SetPosition(Vector2 position) => _bubbleNodeController.SetPosition(position, true);

        public void DestroyComponent()
        {
            _collisionEnterDisposable.Dispose();
            _strikerView.DestroyComponent();
        }

        public class Factory : PlaceholderFactory<string, Vector2, StrikerController>
        {
        }
    }
}