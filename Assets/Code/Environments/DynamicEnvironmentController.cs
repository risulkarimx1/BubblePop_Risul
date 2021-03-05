using System;
using Assets.Code.Signals;
using Zenject;

namespace Assets.Code.Environments
{
    public class DynamicEnvironmentController: IDisposable
    {
        private readonly DynamicEnvironmentView _dynamicEnvironmentView;
        private readonly SignalBus _signalBus;

        public DynamicEnvironmentController( DynamicEnvironmentView dynamicEnvironmentView, SignalBus signalBus)
        {
            _dynamicEnvironmentView = dynamicEnvironmentView;
            _signalBus = signalBus;
            _signalBus.Subscribe<StrikeSignal>(OnStrike);
        }

        private void OnStrike()
        {
            _dynamicEnvironmentView.ShootPiston();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<StrikeSignal>(OnStrike);
        }
    }
}