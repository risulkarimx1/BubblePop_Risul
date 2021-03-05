using System;
using Assets.Code.LevelGeneration;
using Assets.Code.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Code.MenuScene
{
    public class MenuSceneUiController
    {
        private readonly MenuSceneUiView _menuSceneUiView;

        public MenuSceneUiController(MenuSceneUiView menuSceneUiView, LevelDataContext levelDataContext)
        {
            _menuSceneUiView = menuSceneUiView;

            for (int i = 0; i < _menuSceneUiView.LevelButtons.Length; i++)
            {
                var index = i;
                _menuSceneUiView.LevelButtons[i]
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        levelDataContext.SelectedLevel = index;
                        SceneManager.LoadScene(Constants.GameSceneIndex);
                    }).AddTo(_menuSceneUiView);
            }
        }
    }

    public class MenuSceneUiView : MonoBehaviour
    {
        [SerializeField] private Button[] _levelButtons;

        public Button[] LevelButtons => _levelButtons;
    }
}