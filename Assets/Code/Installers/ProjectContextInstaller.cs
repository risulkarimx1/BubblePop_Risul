using Assets.Code.Audio;
using Assets.Code.LevelGeneration;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.Installers
{
    [CreateAssetMenu(fileName = "ProjectContextInstaller", menuName = "Installers/ProjectContextInstaller")]
    public class ProjectContextInstaller : ScriptableObjectInstaller<ProjectContextInstaller>
    {
        [SerializeField] private GameObject _audioController;
        
        public override void InstallBindings()
        {
            Container.Bind<AudioController>().FromComponentInNewPrefab(_audioController).AsSingle().NonLazy();
            Container.Bind<LevelDataContainer>().FromScriptableObjectResource(Constants.LevelDataContainerPath).AsSingle().NonLazy();
            Container.Bind<LevelDataContext>().AsSingle().NonLazy();
        }
    }
}