using Assets.Code.Bubble;
using UnityEngine;
using Zenject;

namespace Assets.Code.Managers
{
    public class GameSceneManager : IInitializable, ITickable
    {
        private readonly BubbleFactory _bubbleBubbleFactory;

        public GameSceneManager(BubbleFactory bubbleBubbleFactory)
        {
            _bubbleBubbleFactory = bubbleBubbleFactory;
        }
        
        public void Initialize()
        {
            
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _bubbleBubbleFactory.Create(BubbleType.Blue, new Coordinate() {Row = 0, Col = 0});
            }
        }
    }
}
