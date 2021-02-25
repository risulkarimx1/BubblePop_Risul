using Assets.Code.Bubble;
using UnityEngine;
using Zenject;

namespace Assets.Code.Managers
{
    public class GameSceneManager : IInitializable, ITickable
    {
        private readonly BubbleFactory _bubbleBubbleFactory;
        
        private readonly BubbleGraph _bubbleGraph;
        private readonly StrikerManager _strikerManager;

        public GameSceneManager(BubbleGraph bubbleGraph, StrikerManager strikerManager, BubbleFactory bubbleBubbleFactory)
        {
            _bubbleGraph = bubbleGraph;
            _strikerManager = strikerManager;
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

            if (Input.GetKeyDown(KeyCode.B))
            {
                _bubbleGraph.InitializeBubbleGraph();
            }
        }
    }
}
