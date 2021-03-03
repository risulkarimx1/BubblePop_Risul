using Assets.Code.Signals;
using Zenject;

namespace Assets.Code.Managers
{
    public class GameStateController
    {
        private readonly SignalBus _signalBus;
        private GameState _currentState;
        
        public GameStateController(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        public GameState CurrentSate
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                _signalBus.Fire(new GameStateChangeSignal(){State = _currentState});
                
            }
        }
        
    }
}