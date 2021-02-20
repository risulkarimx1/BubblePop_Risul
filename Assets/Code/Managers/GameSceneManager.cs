using Assets.Code.Bubble;
using UnityEngine;
using Zenject;

namespace Assets.Code.Managers
{
    public class GameSceneManager : IInitializable, ITickable
    {
        private readonly BubbleFactory _bubbleBubbleFactory;
        private readonly BubbleGraph _bubbleGraph;

        public GameSceneManager(BubbleFactory bubbleBubbleFactory, BubbleGraph bubbleGraph)
        {
            _bubbleBubbleFactory = bubbleBubbleFactory;
            _bubbleGraph = bubbleGraph;
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
