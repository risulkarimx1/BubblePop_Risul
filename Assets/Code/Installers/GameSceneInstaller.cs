using Assets.Code.Bubble;
using Assets.Code.Managers;
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
            Container.Bind<Camera>().WithId(Constants.MainCameraId).FromInstance(Camera.main).AsSingle();

            Container.Bind<BubbleDataContainer>().FromScriptableObjectResource(Constants.BubbleDataContainerPath)
                .AsSingle().NonLazy();

            Container.BindFactory<BubbleType, Coordinate, IBubbleNodeController, BubbleFactory>()
                .FromFactory<BubbleNodeFactory>();

            Container.BindFactory<string, Vector2, StrikerController, StrikerController.Factory>();

            Container.Bind<BubbleGraph>().AsSingle();

            Container.BindInterfacesAndSelfTo<StrikerManager>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<GameSceneManager>().AsSingle();
        }
    }
}