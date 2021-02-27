using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Bubble
{
    [CreateAssetMenu(fileName = "_bubbleDataContainer", menuName = "BubblePop/_bubbleDataContainer", order = 1)]
    public class BubbleDataContainer : ScriptableObject
    {
        [SerializeField] private GameObject _redBubble;
        [SerializeField] private GameObject _blueBubble;
        [SerializeField] private GameObject _greenBubble;

        [Header("Striker Component Settings")] [SerializeField]
        private PhysicsMaterial2D _strikerPhysicsMaterial;

        [SerializeField] private string _strikerSequence;

        public PhysicsMaterial2D StrikerPhysicsMaterial => _strikerPhysicsMaterial;

        public string StrikerSequence => _strikerSequence;

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