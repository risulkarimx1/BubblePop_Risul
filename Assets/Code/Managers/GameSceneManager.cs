using Assets.Code.Bubble;
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
            _ =_bubbleGraph.Initialize();
            _strikerManager.InitializeStrikers();
        }

        public void Tick()
        {
            
        }
    }
}
