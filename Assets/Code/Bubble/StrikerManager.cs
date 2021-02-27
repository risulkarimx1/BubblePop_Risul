using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Bubble
{
    public class StrikerManager : ITickable
    {
        
        private readonly Vector2 _hiddenPosition = new Vector2(8, -12);
        private readonly Vector2 _firstPosition = new Vector2(2, -12);
        private readonly Vector2 _secondPosition = new Vector2(4, -12);
        private StrikerController[] _strikerControllers;
        private readonly StrikerController.Factory _strikerFactory;
        private readonly BubbleDataContainer _bubbleDataContainer;

        private int _currentStriker;
        private int _totalStrikers;

        [Inject(Id = Constants.MainCameraId)] private Camera _mainCamera;

        public StrikerManager(StrikerController.Factory strikerFactory, BubbleDataContainer bubbleDataContainer)
        {
            _strikerFactory = strikerFactory;
            _bubbleDataContainer = bubbleDataContainer;
        }

        public void InitializeStrikers()
        {
            var strikers = _bubbleDataContainer.StrikerSequence.Trim().Split(',');
            _totalStrikers = strikers.Length;
            _strikerControllers = new StrikerController[_totalStrikers];
            for (int i = 0; i < strikers.Length; i++)
            {
                
                _strikerControllers[i] = _strikerFactory.Create(strikers[i], _hiddenPosition);
                _strikerControllers[i].SetName($"Striker- {i}");
            }

            _strikerControllers[0].SetPosition(_firstPosition);
            _strikerControllers[1].SetPosition(_secondPosition);
            _currentStriker = 0;
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0) && _currentStriker < _strikerControllers.Length)
            {
                var mousePosition = Input.mousePosition;
                mousePosition.z = Mathf.Abs(0.0f - _mainCamera.transform.position.z);
                mousePosition = _mainCamera.ScreenToWorldPoint(mousePosition);
                _strikerControllers[_currentStriker].Strike(mousePosition);
                _currentStriker++;
                UpdatePositions();
            }
        }

        public void UpdatePositions()
        {
            if (_currentStriker < _totalStrikers)
            {
                if (_currentStriker == _totalStrikers - 1)
                {
                    _strikerControllers[_currentStriker].SetPosition(_firstPosition);
                }
                else
                {
                    _strikerControllers[_currentStriker].SetPosition(_firstPosition);
                    _strikerControllers[_currentStriker + 1].SetPosition(_secondPosition);
                }
            }
        }
    }
}