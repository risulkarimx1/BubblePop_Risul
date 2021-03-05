using Assets.Code.MenuScene;
using UniRx;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MenuSceneInstaller", menuName = "Installers/MenuSceneInstaller")]
public class MenuSceneInstaller : ScriptableObjectInstaller<MenuSceneInstaller>
{
    [SerializeField] private MenuSceneUiView _menuSceneUiViewPrefab;

    public override void InstallBindings()
    {
        Container.Bind<MenuSceneUiView>().FromComponentInNewPrefab(_menuSceneUiViewPrefab).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MenuSceneUiController>().AsSingle().NonLazy();
    }
}