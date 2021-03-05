using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.LevelGeneration
{
    [CreateAssetMenu(fileName = "LevelDataContainer", menuName = "BubblePop/Level Data Container", order = 1)]
    public class LevelDataContainer: ScriptableObject
    {
        [SerializeField] private List<TextAsset> _levelData;
        [SerializeField] private string [] _strikerData;
        public string GetLevelData(int level)
        {
            return _levelData[level].text;
        }

        public string GetStrikerData(int level)
        {
            return _strikerData[level];
        }
    }
}
