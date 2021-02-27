using UnityEngine;

namespace Assets.Code.Utils
{
    public class Constants
    {
        public const string LevelDataContainerPath = "LevelData/LevelDataContainer";
        public const string BubbleDataContainerPath = "BubbleData/BubbleDataContainer";
        public const string MainCameraId = "MainCamera";
        public static string BubbleTag = "Bubble";
        public static LayerMask BubbleLayerMask = 1 << 9;
    }
}