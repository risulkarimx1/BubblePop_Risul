using UnityEngine;

namespace Assets.Code.Utils
{
    public class Constants
    {
        public const string LevelDataContainerPath = "LevelData/LevelDataContainer";
        public const string BubbleDataContainerPath = "BubbleData/BubbleDataContainer";
        public const string MainCameraId = "MainCamera";
        public static string BubbleTag = "Bubble";
        public static string SideWallTag = "SideWall";
        public static LayerMask BubbleLayerMask = 1 << 9;
        public static string CeilingTag = "Ceiling";

        // Bubble positions
        public static readonly Vector2 HiddenPosition = new Vector2(8, -12);
        public static readonly Vector2 FirstPosition = new Vector2(2, -12);
        public static readonly Vector2 SecondPosition = new Vector2(4, -12);
    }
}