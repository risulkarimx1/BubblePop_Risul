using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Assets.Code.Bubble
{
    [Serializable]
    public class BubbleNodeModel
    {
        [SerializeField]
        private BubbleType _bubbleType;

        public ReactiveProperty<int> NodeValue { get; set; } = new ReactiveProperty<int>(0);

        public BubbleType BubbleType
        {
            get => _bubbleType;
            set => _bubbleType = value;
        }
        
        public BubbleNodeModel(BubbleType bubbleType, int value)
        {
            BubbleType = bubbleType;
            NodeValue.Value = value;
        }

        public override string ToString()
        {
            return $"BubbleType: {BubbleType}";
        }
    }
}