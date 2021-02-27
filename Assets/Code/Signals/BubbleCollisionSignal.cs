using Assets.Code.Bubble;
using UnityEngine;

public class BubbleCollisionSignal
{
    public IBubbleNodeController StrikerNode { get; set; }
    public Collision2D CollisionObject { get; set; }
}