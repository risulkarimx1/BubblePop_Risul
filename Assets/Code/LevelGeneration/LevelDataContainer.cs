using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.LevelGeneration
{
    [CreateAssetMenu(fileName = "LevelDataContainer", menuName = "BubblePop/Level Data Container", order = 1)]
    public class LevelDataContainer: ScriptableObject
    {
        [SerializeField]
        private List<TextAsset> _levelData;

        public string GetLevelData(int level)
        {
            return _levelData[level].text;
        }
    }
}
