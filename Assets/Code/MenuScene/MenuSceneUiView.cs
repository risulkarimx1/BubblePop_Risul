using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.MenuScene
{
    public class MenuSceneUiView : MonoBehaviour
    {
        [SerializeField] private Button[] _levelButtons;

        public Button[] LevelButtons => _levelButtons;
    }
}