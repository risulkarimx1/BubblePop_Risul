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
            Container.Bind<BubblePrefabContainer>().FromScriptableObjectResource(Constants.BubbleDataContainerPath)
                .AsSingle().NonLazy();
            Container.BindFactory<BubbleType, Coordinate, IBubbleNodeController, BubbleFactory>().WithId(Constants.InitialBubbleFactory)
                .FromFactory<BubbleFactoryForInitialGraph>();

            Container.Bind<BubbleGraph>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameSceneManager>().AsSingle();
        }
    }
}