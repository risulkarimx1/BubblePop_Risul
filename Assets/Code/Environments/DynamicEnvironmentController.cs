using System;
using Assets.Code.Signals;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Assets.Code.Environments
{
    public class DynamicEnvironmentController : MonoBehaviour
    {
        [SerializeField] private Transform[] _clouds;
        [SerializeField] private Transform _piston;

        [Inject] private SignalBus _signalBus;
        
        private void Awake()
        {
            foreach (var cloud in _clouds)
            {
                var seq = DOTween.Sequence();
                seq.Append(cloud.DOMoveX(-3, Random.Range(30, 150))).SetRelative(false);
                seq.SetLoops(-1);
            }
            _signalBus.Subscribe<StrikeSignal>(ShootPiston);
        }

        public void ShootPiston()
        {
            DOTween.Sequence()
                .Append(_piston.DOMoveY(-12, .05f)).SetEase(Ease.OutBounce)
                .AppendInterval(.5f)
                .Append(_piston.DOMoveY(-13, 1));
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<StrikeSignal>(ShootPiston);
        }
    }
}