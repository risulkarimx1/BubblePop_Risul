using System;
using Assets.Code.Audio;
using Assets.Code.Bubble;
using Assets.Code.Signals;
using DG.Tweening;
using UniRx.Async;
using Zenject;

namespace Assets.Code.Managers
{
    public class GameSceneManager : IInitializable, IDisposable
    {
        private readonly BubbleGraph _bubbleGraph;
        private readonly StrikerManager _strikerManager;
        private readonly SignalBus _signalBus;
        private readonly AudioController _audioController;

        public GameSceneManager(BubbleGraph bubbleGraph,
            StrikerManager strikerManager, SignalBus signalBus,
            AudioController audioController)
        {
            _bubbleGraph = bubbleGraph;
            _strikerManager = strikerManager;
            _signalBus = signalBus;
            _audioController = audioController;
            _signalBus.Subscribe<GameStateChangeSignal>(OnGameStateChanged);
            _audioController.PlayGameBg();
        }

        private void OnGameStateChanged(GameStateChangeSignal gameStateChangeSignal)
        {
            if (gameStateChangeSignal.State == GameState.GameOverWin)
            {
                _audioController.PlayWin();
            }
            else if (gameStateChangeSignal.State == GameState.GameOverLose)
            {
                _audioController.PlayLose();
            }
        }

        public void Initialize()
        {
            _ = InitializeAsync();
        }

        private async UniTask InitializeAsync()
        {
            await _bubbleGraph.InitializeAsync();
            _strikerManager.InitializeStrikers();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GameStateChangeSignal>(OnGameStateChanged);
            DOTween.KillAll();
        }
    }
}