using Assets.Code.LevelGeneration;
using Assets.Code.Managers;
using Assets.Code.Signals;
using Assets.Code.Utils;
using Zenject;

namespace Assets.Code.Bubble
{
    public class StrikerManager
    {
        private StrikerController[] _strikerControllers;
        private readonly StrikerController.Factory _strikerFactory;
        private readonly SignalBus _signalBus;
        private readonly GameStateController _gameStateController;
        private readonly LevelDataContext _levelDataContext;

        private int _currentStriker;
        private int _totalStrikers;

        public StrikerManager(StrikerController.Factory strikerFactory,
            SignalBus signalBus, GameStateController gameStateController, LevelDataContext levelDataContext)
        {
            _strikerFactory = strikerFactory;
            _signalBus = signalBus;
            _gameStateController = gameStateController;
            _levelDataContext = levelDataContext;
            _signalBus.Subscribe<StrikeSignal>(OnStrike);
        }

        private void OnStrike(StrikeSignal strikerSignal)
        {
            if (_gameStateController.CurrentSate == GameState.WaitingToShoot)
            {
                if(_currentStriker >= _totalStrikers) return;
                
                _gameStateController.CurrentSate = GameState.Shooting;
                _strikerControllers[_currentStriker].Strike(strikerSignal.Direction);
                _currentStriker++;
                UpdatePositions();
                if(_currentStriker == _totalStrikers) _signalBus.Fire<StrikerFinishedSignal>();
            }
        }

        public void InitializeStrikers()
        {
            var strikers = _levelDataContext.GetSelectedLevelStrikerData().Trim().Split(',');
            _totalStrikers = strikers.Length;
            _strikerControllers = new StrikerController[_totalStrikers];
            for (int i = 0; i < strikers.Length; i++)
            {
                if (i == 0)
                {
                    _strikerControllers[i] = _strikerFactory.Create(strikers[i], Constants.FirstPosition);
                }else if (i == 1)
                {
                    _strikerControllers[i] = _strikerFactory.Create(strikers[i], Constants.SecondPosition);
                }
                else
                {
                    _strikerControllers[i] = _strikerFactory.Create(strikers[i], Constants.HiddenPosition);
                }
                
                _strikerControllers[i].SetName($"Striker- {i}");
            }
            
            _currentStriker = 0;
        }

        public void UpdatePositions()
        {
            if (_currentStriker < _totalStrikers)
            {
                if (_currentStriker == _totalStrikers - 1)
                {
                    _strikerControllers[_currentStriker].SetPosition(Constants.FirstPosition);
                }
                else
                {
                    _strikerControllers[_currentStriker].SetPosition(Constants.FirstPosition);
                    _strikerControllers[_currentStriker + 1].SetPosition(Constants.SecondPosition);
                }
            }
        }
    }
}