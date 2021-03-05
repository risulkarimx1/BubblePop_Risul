using System;
using Assets.Code.Audio;
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
        private readonly AudioController _audioController;

        public MenuSceneUiController(MenuSceneUiView menuSceneUiView, LevelDataContext levelDataContext, AudioController audioController)
        {
            _menuSceneUiView = menuSceneUiView;
            _audioController = audioController;

            for (int i = 0; i < _menuSceneUiView.LevelButtons.Length; i++)
            {
                var index = i;
                _menuSceneUiView.LevelButtons[i]
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        _audioController.ButtonClick();
                        levelDataContext.SelectedLevel = index;
                        SceneManager.LoadScene(Constants.GameSceneIndex);
                    }).AddTo(_menuSceneUiView);
            }

            if (audioController.IsPlayingMenuAudio() == false)
            {
                audioController.PlayMenuBg();
            }
        }
    }

    public class MenuSceneUiView : MonoBehaviour
    {
        [SerializeField] private Button[] _levelButtons;

        public Button[] LevelButtons => _levelButtons;
    }
}