using System;
using Assets.Code.LevelGeneration;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.MenuScene
{
    public class MenuSceneUiController: IDisposable
    {
        private readonly MenuSceneUiView _menuSceneUiView;
        private readonly CompositeDisposable _disposable;

        public MenuSceneUiController(MenuSceneUiView menuSceneUiView, LevelDataContext levelDataContext,
            CompositeDisposable disposable)
        {
            _menuSceneUiView = menuSceneUiView;
            _disposable = disposable;
            _menuSceneUiView.Level1Button.OnClickAsObservable().Subscribe(_ =>
            {
                levelDataContext.SelectedLevel = 0;
                Debug.Log($"{levelDataContext.GetSelectedLevelData()}");
            }).AddTo(disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class MenuSceneUiView : MonoBehaviour
    {
        [SerializeField] private Button _level1Button;

        public Button Level1Button => _level1Button;
    }
}