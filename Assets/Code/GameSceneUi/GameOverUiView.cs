using System;
using Assets.Code.Managers;
using Assets.Code.Signals;
using Assets.Code.Utils;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Code.GameSceneUi
{
    public class GameOverUiController: IDisposable
    {
        private readonly GameOverUiView _gameOverView;
        private readonly SignalBus _signalBus;
        private readonly ScoreUiController _scoreUiController;

        public GameOverUiController(GameOverUiView gameOverView, SignalBus signalBus, ScoreUiController scoreUiController)
        {
            _gameOverView = gameOverView;
            _signalBus = signalBus;
            _scoreUiController = scoreUiController;
            _gameOverView.HomeButton.OnClickAsObservable().Subscribe(_ =>
            {
                SceneManager.LoadScene(Constants.MenuSceneIndex);
            }).AddTo(_gameOverView);

            _gameOverView.PlayButton.OnClickAsObservable().Subscribe(_ =>
            {
                _gameOverView.PlayButtonPressed(() =>
                {
                    SceneManager.LoadScene(Constants.GameSceneIndex);
                });
            }).AddTo(_gameOverView);
            
            _signalBus.Subscribe<GameStateChangeSignal>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangeSignal gameStateChangeSignal)
        {
            if (gameStateChangeSignal.State == GameState.GameOverWin)
            {
                _gameOverView.ShowGameOver(true, _scoreUiController.Score);
            }else if (gameStateChangeSignal.State == GameState.GameOverLose)
            {
                _gameOverView.ShowGameOver(false, _scoreUiController.Score);
            }
        }

        public void Dispose()
        {
            
        }
    }
    
    public class GameOverUiView: MonoBehaviour
    {
        [SerializeField] private Transform _bg;
        
        [SerializeField] private Transform _gameWinLoseTextTransform;
        [SerializeField] private Transform _scoreTextTransform;
        [SerializeField] private Transform _homeButtonTransform;
        [SerializeField] private Transform _playButtonTransform;

        private TextMeshProUGUI _gameWinLoseText;
        private TextMeshProUGUI _scoreText;
        private Button _homeButton;
        private Button _playButton;

        public Button HomeButton => _homeButton;

        public Button PlayButton => _playButton;

        private void Awake()
        {
            _gameWinLoseText = _gameWinLoseTextTransform.GetComponent<TextMeshProUGUI>();
            _scoreText = _scoreTextTransform.GetComponentInChildren<TextMeshProUGUI>();
            _homeButton = _homeButtonTransform.GetComponent<Button>();
            _playButton = _playButtonTransform.GetComponent<Button>();
            
            _gameWinLoseTextTransform.localScale = Vector3.zero;
            _scoreTextTransform.localScale = Vector3.zero;
            _homeButtonTransform.localScale = Vector3.zero;
            _playButtonTransform.localScale = Vector3.zero;
        }

        public void ShowGameOver(bool win, int score)
        {
            _bg.gameObject.SetActive(true);
            _gameWinLoseText.text = win ? "Perfect!!!" : "Game Over!";
            _scoreText.text = score.ToString();
            AnimateUiObject(Vector3.one, Ease.InOutBounce);
        }

        private void AnimateUiObject(Vector3 scale, Ease ease, TweenCallback callback = null)
        {
            DOTween.Sequence()
                .Append(_gameWinLoseTextTransform.DOScale(scale, 0.25f).SetEase(ease))
                .Append(_scoreTextTransform.DOScale(scale, 0.25f).SetEase(ease))
                .Append(_homeButtonTransform.DOScale(scale, 0.25f).SetEase(ease))
                .Join(_playButtonTransform.DOScale(scale, 0.25f).SetEase(ease))
                .AppendCallback(callback);
        }

        public void PlayButtonPressed(TweenCallback callback)
        {
            AnimateUiObject(Vector3.zero, Ease.InOutBounce, callback);
        }
    }
}