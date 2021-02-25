using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Bubble
{
    public class StrikerManager : ITickable
    {
        private readonly Vector2 _hiddenPosition = new Vector2(-1000, -1000);
        private readonly Vector2 _firstPosition = new Vector2(2, -10);
        private readonly Vector2 _secondPosition = new Vector2(4, -10);
        private readonly Vector2 _thirdPosition = new Vector2(6, -10);
        private StrikerController[] _strikerControllers;
        private readonly StrikerController.Factory _strikerFactory;

        private int _currentStriker;

        [Inject(Id = Constants.MainCameraId)] private Camera _mainCamera;

        public StrikerManager(StrikerController.Factory strikerFactory, BubbleDataContainer bubbleDataContainer)
        {
            _strikerFactory = strikerFactory;
            CreateStriker(bubbleDataContainer.StrikerSequence);
        }

        public void CreateStriker(string strikerSequence)
        {
            var strikers = strikerSequence.Trim().Split(',');
            _strikerControllers = new StrikerController[strikers.Length];
            for (int i = 0; i < strikers.Length; i++)
            {
                _strikerControllers[i] = _strikerFactory.Create(strikers[i], _hiddenPosition);
            }

            _strikerControllers[0].SetPosition(_firstPosition);
            _strikerControllers[1].SetPosition(_secondPosition);
            _currentStriker = 0;
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = Input.mousePosition;
                mousePosition.z = Mathf.Abs(0.0f - _mainCamera.transform.position.z);
                mousePosition = _mainCamera.ScreenToWorldPoint(mousePosition);
                _strikerControllers[_currentStriker].Strike(mousePosition);
                _currentStriker++;
            }
        }
    }
}