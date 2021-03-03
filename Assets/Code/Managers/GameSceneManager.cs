using System;
using Assets.Code.Bubble;
using Assets.Code.Signals;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace Assets.Code.Managers
{
    public class GameSceneManager : IInitializable, IDisposable
    {
        private readonly BubbleGraph _bubbleGraph;
        private readonly StrikerManager _strikerManager;
        private readonly SignalBus _signalBus;

        private bool _isStrikerFinished = false;
        public GameSceneManager(BubbleGraph bubbleGraph, StrikerManager strikerManager, SignalBus signalBus)
        {
            _bubbleGraph = bubbleGraph;
            _strikerManager = strikerManager;
            _signalBus = signalBus;
            _signalBus.Subscribe<GameStateChangeSignal>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangeSignal gameStateChangeSignal)
        {
            if (gameStateChangeSignal.State == GameState.GameOverWin)
            {
                Debug.Log($"Perfect: Show Menu to go back home or load again");
            }
            else if (gameStateChangeSignal.State == GameState.GameOverLose)
            {
                Debug.Log($"Lose!! .. Show menu to play the game again");
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
        }
    }
}
