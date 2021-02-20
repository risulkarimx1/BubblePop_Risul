using Assets.Code.Bubble;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Managers
{
    public class GameSceneManager : IInitializable, ITickable
    {
        [Inject(Id = Constants.InitialBubbleFactory)]
        private BubbleFactory _bubbleBubbleFactory;
        
        private readonly BubbleGraph _bubbleGraph;

        public GameSceneManager(BubbleGraph bubbleGraph)
        {
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
