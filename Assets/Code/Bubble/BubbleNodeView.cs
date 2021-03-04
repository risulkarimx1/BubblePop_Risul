using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UniRx.Async;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _valueText;
        public List<string> Neighbors; 
        private Transform _transform;
        private CircleCollider2D _circleCollider;
        public TextMeshPro ValueText => _valueText;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _circleCollider = GetComponent<CircleCollider2D>();

        }

        public async UniTask SetPositionAsync(Vector2 position, bool animate = false, float speed = 1, TweenCallback callback = null, Ease ease = Ease.Linear)
        {
            if (animate)
            {
                if (callback != null)
                {
                    await DOTween.Sequence()
                        .Append(_transform.DOMove(position, 1 / speed)).SetEase(ease)
                        .AppendCallback(callback).AsyncWaitForCompletion().ConfigureAwait(false);
                }
                else
                {
                    await _transform.DOMove(position, 1 / speed).SetEase(ease).AsyncWaitForCompletion().ConfigureAwait(false);
                }
            }
            else
            {
                _transform.position = position;
            }
            
        }
        
        public void SetPosition(Vector2 position, bool animate = false, float speed = 1, TweenCallback callback = null, Ease ease = Ease.Linear)
        {
            if (animate)
            {
                if (callback != null)
                {
                    DOTween.Sequence()
                        .Append(_transform.DOMove(position, 1 / speed)).SetEase(ease)
                        .AppendCallback(callback);
                }
                else
                {
                    _transform.DOMove(position, 1 / speed).SetEase(ease);
                }
            }
            else
            {
                _transform.position = position;
            }
        }

        public StrikerView ConvertToStriker() => gameObject.AddComponent<StrikerView>();

        public Vector3 GetPosition()
        {
            return _transform.position;
        }

        public void AnimateHide(TweenCallback callback)
        {
            if (callback == null)
            {
                _transform.DOScale(Vector2.zero, 0.1f);
            }
            else
            {
                DOTween.Sequence()
                    .Append(_transform.DOScale(Vector2.zero, 0.1f))
                    .AppendCallback(callback);
            }
        }

        public void DropNode()
        {
            _circleCollider.enabled = false;
        }
        
        public void Remove()
        {
            gameObject.SetActive(false);
        }
    }
}