using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Code.GameSceneUi
{
    public class ScoreUiView : MonoBehaviour
    {
        [SerializeField] private Transform _imageIcon;
        [SerializeField] private TextMeshProUGUI _scoreText;

        public void SetScore(int score)
        {
            _imageIcon.DORotate(Vector3.up * 360, 1); 
            _scoreText.text = score.ToString();
        }
    }
}
