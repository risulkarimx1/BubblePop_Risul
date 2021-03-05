using Assets.Code.Bubble;
using Assets.Code.Environments;
using Assets.Code.GameSceneUi;
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
        [SerializeField] private GameObject _dynamicEnvironment;
        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private GameObject _scoreUiPrefab;
        [SerializeField] private GameObject _gameOverUiPrefab;

        public override void InstallBindings()
        {
            // camera
            Container.Bind<CameraEffectsController>().FromComponentInNewPrefab(_mainCamera).AsSingle();

            // signals
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameStateChangeSignal>();
            Container.DeclareSignal<BubbleCollisionSignal>();
            Container.DeclareSignal<CeilingCollisionSignal>();
            Container.DeclareSignal<StrikerFinishedSignal>();
            Container.DeclareSignal<StrikeSignal>();
            Container.DeclareSignal<ScoreUpdateSignal>();

            // Game States
            Container.Bind<GameStateController>().AsSingle();

            // Data Container
            Container.Bind<BubbleDataContainer>().FromScriptableObjectResource(Constants.BubbleDataContainerPath)
                .AsSingle().NonLazy();

            // Explosion Factory
            Container.BindFactory<Vector3, ExplosionController, ExplosionController.Factory>().FromPoolableMemoryPool(
                x => x.WithInitialSize(10).FromComponentInNewPrefab(_explosionPrefab).UnderTransformGroup("Explosions"));

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

            // Environment

            Container.Bind<DynamicEnvironmentView>().FromComponentInNewPrefab(_dynamicEnvironment).AsSingle();
            Container.BindInterfacesAndSelfTo<DynamicEnvironmentController>().AsSingle();

            // Ui
            Container.Bind<ScoreUiView>().FromComponentInNewPrefab(_scoreUiPrefab).AsSingle();
            Container.Bind<ScoreUiModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreUiController>().AsSingle();

            Container.Bind<GameOverUiView>().FromComponentInNewPrefab(_gameOverUiPrefab).AsSingle();
            Container.BindInterfacesAndSelfTo<GameOverUiController>().AsSingle();
            
            // Mouse Input
            Container.Bind<MouseShootView>().FromComponentInNewPrefab(_mouseShootView).AsSingle();
            Container.BindInterfacesAndSelfTo<MouseShootController>().AsSingle().NonLazy();
            // Managers
            Container.BindInterfacesAndSelfTo<StrikerManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameSceneManager>().AsSingle();
        }
    }
}