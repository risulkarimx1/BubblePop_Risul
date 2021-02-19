using UnityEngine;

namespace Assets.Code.Bubble
{
    [CreateAssetMenu(fileName = "BubbleDataContainer", menuName = "BubblePop/BubblePrefabContainer", order = 1)]
    public class BubblePrefabContainer: ScriptableObject
    {
        [SerializeField] private GameObject _redBubble;
        [SerializeField] private GameObject _blueBubble;
        [SerializeField] private GameObject _greenBubble;

        public GameObject GetBubbleOfType(BubbleType bubbleType)
        {
            switch (bubbleType)
            {
                case BubbleType.Green:
                    return _greenBubble;
                case BubbleType.Blue:
                    return _blueBubble;
                case BubbleType.Red:
                    return _redBubble;
                default:
                    return _redBubble;
            }
        }
    }
}