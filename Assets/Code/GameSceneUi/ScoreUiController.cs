using System;
using Assets.Code.Signals;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Assets.Code.GameSceneUi
{
    public class ScoreUiController : IDisposable
    {
        private readonly ScoreUiModel _scoreUiModel;
        private readonly ScoreUiView _scoreUiView;
        private readonly SignalBus _signalBus;

        public int Score => _scoreUiModel.Score.Value;

        public ScoreUiController(ScoreUiModel scoreUiModel, ScoreUiView scoreUiView, SignalBus signalBus)
        {
            _scoreUiModel = scoreUiModel;
            _scoreUiView = scoreUiView;
            _signalBus = signalBus;
            _scoreUiModel.Score.Subscribe(score => { _scoreUiView.SetScore(score); })
                .AddTo(_scoreUiView);
            _signalBus.Subscribe<ScoreUpdateSignal>(OnScoreUpdated);
        }

        private void OnScoreUpdated(ScoreUpdateSignal obj)
        {
            _ = UpdateScoreAsync(obj.Score);
        }

        private async UniTask UpdateScoreAsync(int score)
        {
            var targetScore = _scoreUiModel.Score.Value + score;
            while (_scoreUiModel.Score.Value < targetScore)
            {
                _scoreUiModel.Score.Value += 10;
                await UniTask.DelayFrame(1);
            }

            _scoreUiModel.Score.Value = targetScore;
        }

        public void Hide() => _scoreUiView.Hide();

        public void Dispose()
        {
            _signalBus.Unsubscribe<ScoreUpdateSignal>(OnScoreUpdated);
        }
    }
}