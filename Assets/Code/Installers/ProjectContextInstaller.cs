using Assets.Code.LevelGeneration;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Installers
{
    [CreateAssetMenu(fileName = "ProjectContextInstaller", menuName = "Installers/ProjectContextInstaller")]
    public class ProjectContextInstaller : ScriptableObjectInstaller<ProjectContextInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<LevelDataContainer>().FromScriptableObjectResource(Constants.LevelDataContainerPath).AsSingle().NonLazy();
            Container.Bind<LevelDataContext>().AsSingle().NonLazy();
        }
    }
}