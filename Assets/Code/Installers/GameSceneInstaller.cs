using Assets.Code.Bubble;
using Assets.Code.Managers;
using Assets.Code.Signals;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Installers
{
    [CreateAssetMenu(fileName = "GameSceneInstaller", menuName = "Installers/GameSceneInstaller")]
    public class GameSceneInstaller : ScriptableObjectInstaller<GameSceneInstaller>
    {
        public override void InstallBindings()
        {
            // signals
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<BubbleCollisionSignal>();
            Container.DeclareSignal<CeilingCollisionSignal>();
            
            // camera
            Container.Bind<Camera>().WithId(Constants.MainCameraId).FromInstance(Camera.main).AsSingle();

            // Data Container
            Container.Bind<BubbleDataContainer>().FromScriptableObjectResource(Constants.BubbleDataContainerPath)
                .AsSingle().NonLazy();

            // Bubble Factory
            Container.BindFactory<string, IBubbleNodeController, BubbleFactory>()
                .FromFactory<BubbleNodeFactory>();

            // Striker Bubble Factory
            Container.BindFactory<string, Vector2, StrikerController, StrikerController.Factory>();

            // Bubble Graph
            Container.Bind<NodeIsolationHelper>().AsSingle();
            Container.Bind<NumericMergeHelper>().AsSingle();
            Container.Bind<BubbleAttachmentHelper>().AsSingle();
            Container.Bind<BubbleGraph>().AsSingle();

            // Managers
            Container.BindInterfacesAndSelfTo<StrikerManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameSceneManager>().AsSingle();
        }
    }
}