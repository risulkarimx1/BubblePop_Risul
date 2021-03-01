using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Code.Bubble
{
    public class BubbleNodeView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _valueText;
        public List<string> Neighbors; 
        private Transform _transform;

        public TextMeshPro ValueText => _valueText;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            
        }

        public void SetPosition(Vector2 position, bool animate = false, float speed = 1, TweenCallback callback = null)
        {
            if (animate)
            {
                if (callback != null)
                {
                    DOTween.Sequence()
                        .Append(_transform.DOMove(position, 1 / speed))
                        .AppendCallback(callback);
                }
                else
                {
                    _transform.DOMove(position, 1 / speed);
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

        public void Remove()
        {
            gameObject.SetActive(false);
        }
    }
}