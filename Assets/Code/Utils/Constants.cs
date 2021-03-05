using UnityEngine;

namespace Assets.Code.Utils
{
    public static class Constants
    {
        public const string LevelDataContainerPath = "LevelData/LevelDataContainer";
        public const string BubbleDataContainerPath = "BubbleData/BubbleDataContainer";
        public const string BubbleTag = "Bubble";
        public const string SideWallTag = "SideWall";
        public static readonly LayerMask BubbleLayerMask = 1 << 9;
        public static readonly LayerMask InputEffectsMask = 3 << 9;
        public static readonly string CeilingTag = "Ceiling";

        // Bubble positions
        public static readonly Vector2 HiddenPosition = new Vector2(8, -12);
        public static readonly Vector2 FirstPosition = new Vector2(2, -12);
        public static readonly Vector2 SecondPosition = new Vector2(4, -12);

        public const int MaxBubbleValue = 2048;
        public static int MenuSceneIndex = 0;

        public static int GameSceneIndex = 1;
    }
}