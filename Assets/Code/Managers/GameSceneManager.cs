using Assets.Code.Bubble;
using UniRx.Async;
using Zenject;

namespace Assets.Code.Managers
{
    public class GameSceneManager : IInitializable
    {
        private readonly BubbleGraph _bubbleGraph;
        private readonly StrikerManager _strikerManager;

        public GameSceneManager(BubbleGraph bubbleGraph, StrikerManager strikerManager)
        {
            _bubbleGraph = bubbleGraph;
            _strikerManager = strikerManager;
        }
        
        public void Initialize()
        {
            _ = InitializeAsync();
        }

        private async UniTask InitializeAsync()
        {
            await _bubbleGraph.Initialize();
            _strikerManager.InitializeStrikers();
        }
    }
}
