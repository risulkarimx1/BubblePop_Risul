using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Bubble
{
    public class StrikerManager : ITickable
    {
        private StrikerController[] _strikerControllers;
        private readonly StrikerController.Factory _strikerFactory;
        private readonly BubbleDataContainer _bubbleDataContainer;
        private readonly CameraEffects _cameraEffects;

        private int _currentStriker;
        private int _totalStrikers;

        public StrikerManager(StrikerController.Factory strikerFactory, BubbleDataContainer bubbleDataContainer,
            CameraEffects cameraEffects)
        {
            _strikerFactory = strikerFactory;
            _bubbleDataContainer = bubbleDataContainer;
            _cameraEffects = cameraEffects;
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

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0) && _currentStriker < _strikerControllers.Length)
            {
                // var mousePosition = Input.mousePosition;
                // mousePosition.z = Mathf.Abs(0.0f - _cameraEffects.MainCamera.transform.position.z);
                // mousePosition = _cameraEffects.MainCamera.ScreenToWorldPoint(mousePosition);
                // _strikerControllers[_currentStriker].Strike(mousePosition);
                // _currentStriker++;
                // UpdatePositions();
            }
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