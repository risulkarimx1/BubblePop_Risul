﻿using Assets.Code.Bubble;
using UnityEngine;

namespace Assets.Code.Signals
{
    public class BubbleCollisionSignal
    {
        public IBubbleNodeController StrikerNode { get; set; }
        public Collision2D CollisionObject { get; set; }
    }
}