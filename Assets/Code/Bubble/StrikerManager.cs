using Assets.Code.Signals;
using Assets.Code.Utils;
using Zenject;

namespace Assets.Code.Bubble
{
    public class StrikerManager
    {
        private StrikerController[] _strikerControllers;
        private readonly StrikerController.Factory _strikerFactory;
        private readonly BubbleDataContainer _bubbleDataContainer;
        private readonly SignalBus _signalBus;

        private int _currentStriker;
        private int _totalStrikers;

        public StrikerManager(StrikerController.Factory strikerFactory, BubbleDataContainer bubbleDataContainer,
            SignalBus signalBus)
        {
            _strikerFactory = strikerFactory;
            _bubbleDataContainer = bubbleDataContainer;
            _signalBus = signalBus;
            _signalBus.Subscribe<StrikeSignal>(OnStrike);
        }

        private void OnStrike(StrikeSignal strikerSignal)
        {
            _strikerControllers[_currentStriker].Strike(strikerSignal.Direction);
            _currentStriker++;
            UpdatePositions();
        }

        public void InitializeStrikers()
        {
            var strikers = _bubbleDataContainer.StrikerSequence.Trim().Split(',');
            _totalStrikers = strikers.Length;
            _strikerControllers = new StrikerController[_totalStrikers];
            for (int i = 0; i < strikers.Length; i++)
            {
                _strikerControllers[i] = _strikerFactory.Create(strikers[i], Constants.HiddenPosition);
                _strikerControllers[i].SetName($"Striker- {i}");
            }

            _strikerControllers[0].SetPosition(Constants.FirstPosition);
            _strikerControllers[1].SetPosition(Constants.SecondPosition);
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