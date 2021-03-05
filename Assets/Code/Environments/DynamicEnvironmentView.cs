using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Code.Environments
{
    public class DynamicEnvironmentView: MonoBehaviour
    {
        [SerializeField] private Transform[] _clouds;
        [SerializeField] private Transform _piston;
        private Sequence _cloudSequence;
        private void Awake()
        {
            foreach (var cloud in _clouds)
            {
                _cloudSequence  = DOTween.Sequence();
                _cloudSequence.Append(cloud.DOMoveX(-3, Random.Range(30, 150))).SetRelative(false);
                _cloudSequence.SetLoops(-1);
            }
        }

        public void ShootPiston()
        {
            DOTween.Sequence()
                .Append(_piston.DOMoveY(-12, .05f)).SetEase(Ease.OutBounce)
                .AppendInterval(.5f)
                .Append(_piston.DOMoveY(-13, 1)).SetAutoKill();
        }

        private void OnDestroy()
        {
            _cloudSequence?.Kill();
        }
    }
}