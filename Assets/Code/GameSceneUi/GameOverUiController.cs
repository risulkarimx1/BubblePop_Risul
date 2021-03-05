using System;
using Assets.Code.Audio;
using Assets.Code.Managers;
using Assets.Code.Signals;
using Assets.Code.Utils;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Code.GameSceneUi
{
    public class GameOverUiController: IDisposable
    {
        private readonly GameOverUiView _gameOverView;
        private readonly SignalBus _signalBus;
        private readonly ScoreUiController _scoreUiController;

        public GameOverUiController(
            GameOverUiView gameOverView, 
            SignalBus signalBus, 
            ScoreUiController scoreUiController,
            AudioController audioController)
        {
            _gameOverView = gameOverView;
            _signalBus = signalBus;
            _scoreUiController = scoreUiController;
            _gameOverView.HomeButton.OnClickAsObservable().Subscribe(_ =>
            {
                audioController.ButtonClick();
                SceneManager.LoadScene(Constants.MenuSceneIndex);
            }).AddTo(_gameOverView);

            _gameOverView.PlayButton.OnClickAsObservable().Subscribe(_ =>
            {
                _gameOverView.PlayButtonPressed(() =>
                {
                    audioController.ButtonClick();
                    SceneManager.LoadScene(Constants.GameSceneIndex);
                });
            }).AddTo(_gameOverView);
            
            _signalBus.Subscribe<GameStateChangeSignal>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangeSignal gameStateChangeSignal)
        {
            if (gameStateChangeSignal.State == GameState.GameOverWin)
            {
                _scoreUiController.Hide();
                _gameOverView.ShowGameOver(true, _scoreUiController.Score);
            }else if (gameStateChangeSignal.State == GameState.GameOverLose)
            {
                _scoreUiController.Hide();
                _gameOverView.ShowGameOver(false, _scoreUiController.Score);
            }
        }

        public void Dispose()
        {
            
        }
    }
}