using UnityEngine;

namespace Assets.Code.Bubble
{
    public static class BubbleUtility
    {
        public static Vector2 GetRange(this float angle)
        {
            // 11.25 is 90/2 = 45 /2 = 22.5/ 2 = 11.25
            return new Vector2(angle - 11.25f, angle + 11.25f);
        }

        public static bool InBetween(this float angle, Vector2 range)
        {
            return InBetween(angle, range.x, range.y);
        }

        public static bool InBetween(this float angle, float min, float max)
        {
            return min <= angle && angle < max;
        }


        public static BubbleType ConvertColorToBubbleType(string color)
        {
            switch (color)
            {
                case "r":
                    return BubbleType.Red;
                case "g":
                    return BubbleType.Green;
                case "b":
                    return BubbleType.Blue;
                case "e":
                default:
                    return BubbleType.Empty;
            }
        }
    }
}