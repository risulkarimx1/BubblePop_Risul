using Assets.Code.Bubble;
using Assets.Code.Managers;
using Assets.Code.ShootEffect;
using Assets.Code.Signals;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Installers
{
    [CreateAssetMenu(fileName = "GameSceneInstaller", menuName = "Installers/GameSceneInstaller")]
    public class GameSceneInstaller : ScriptableObjectInstaller<GameSceneInstaller>
    {
        [SerializeField] private GameObject _mainCamera;
        [SerializeField] private GameObject _mouseShootView;

        public override void InstallBindings()
        {
            // camera
            Container.Bind<CameraEffects>().FromComponentInNewPrefab(_mainCamera).AsSingle();

            // signals
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameStateChangeSignal>();
            Container.DeclareSignal<BubbleCollisionSignal>();
            Container.DeclareSignal<CeilingCollisionSignal>();
            Container.DeclareSignal<StrikerFinishedSignal>();
            Container.DeclareSignal<StrikeSignal>();

            // Game States
            Container.Bind<GameStateController>().AsSingle();

            // Data Container
            Container.Bind<BubbleDataContainer>().FromScriptableObjectResource(Constants.BubbleDataContainerPath)
                .AsSingle().NonLazy();

            // Bubble Factory
            Container.BindFactory<string, IBubbleNodeController, BubbleFactory>()
                .FromFactory<BubbleNodeFactory>();

            // Striker Bubble Factory
            Container.BindFactory<string, Vector2, StrikerController, StrikerController.Factory>();

            // Bubble Graph
            Container.Bind<ColorMergeHelper>().AsSingle();
            Container.Bind<NodeIsolationHelper>().AsSingle();
            Container.Bind<NumericMergeHelper>().AsSingle();
            Container.Bind<BubbleAttachmentHelper>().AsSingle();
            Container.Bind<BubbleGraph>().AsSingle();

            // Mouse Input
            Container.Bind<MouseShootView>().FromComponentInNewPrefab(_mouseShootView).AsSingle();
            Container.BindInterfacesAndSelfTo<MouseShootController>().AsSingle().NonLazy();
            // Managers
            Container.BindInterfacesAndSelfTo<StrikerManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameSceneManager>().AsSingle();
        }
    }
}