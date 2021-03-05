using DG.Tweening;
using UnityEngine;

namespace Assets.Code.Audio
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource _bg;
        [SerializeField] private AudioSource _fx;
        [SerializeField] private AudioSource _longFx;

        [Header("Fx")] [SerializeField] private AudioClip _buttonClick;
        [SerializeField] private AudioClip _bubbleCollide;
        [SerializeField] private AudioClip _explosion;
        [SerializeField] private AudioClip _strike;
        [SerializeField] private AudioClip _numberMerge;
        
        [Header("Long Fx")] 
        [SerializeField] private AudioClip _win;
        [SerializeField] private AudioClip _lose;

        [Header("BG")] [SerializeField] private AudioClip _gameClip;
        [SerializeField] private AudioClip _menuClip;

        // Fx
        public void ButtonClick() => PlayFx(_buttonClick);
        public void BubbleCollide() => PlayFx(_bubbleCollide);
        public void Explosion() => PlayFx(_explosion);
        public void Strike() => PlayFx(_strike);
        public void NumberMerge() => PlayFx(_numberMerge);

        // Long Fx
        public void PlayWin() => PlayLongFx(_win);
        public void PlayLose() => PlayLongFx(_lose);

        // BG
        public void PlayGameBg() => PlayBg(_gameClip);
        public void PlayMenuBg() => PlayBg(_menuClip);

        public bool IsPlayingMenuAudio()
        {
            if (_bg.isPlaying && _bg.clip == _menuClip)
            {
                return true;
            }

            return false;
        }

        private void PlayLongFx(AudioClip clip)
        {
            DOTween.Sequence()
                .Append(_bg.DOFade(0, 0.25f))
                .AppendCallback(() =>
                {
                    _longFx.clip = clip;
                    _longFx.Play();
                })
                .AppendInterval(clip.length)
                .AppendCallback(() =>
                {
                    PlayBg(_menuClip);
                });
        }

        private void PlayFx(AudioClip clip)
        {
            _fx.clip = clip;
            _fx.Play();
        }

        public void PlayBg(AudioClip clip)
        {
            DOTween.Sequence()
                .Append(_bg.DOFade(0, 0.5f))
                .AppendCallback(() =>
                {
                    _bg.Stop();
                    _bg.clip = clip;
                    _bg.DOFade(1, 0.25f);
                    _bg.Play();
                });
        }
    }
}