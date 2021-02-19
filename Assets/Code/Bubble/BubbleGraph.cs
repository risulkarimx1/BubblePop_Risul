using System.Collections.Generic;
using Assets.Code.LevelGeneration;

namespace Assets.Code.Bubble
{
    public class BubbleGraph
    {
        private Dictionary<Coordinate, BubbleNodeModel> _bubbles;

        public BubbleGraph(LevelDataContext levelDataContext)
        {
            _bubbles = new Dictionary<Coordinate, BubbleNodeModel>();
            InitializeBubbleGraph(levelDataContext.GetSelectedLevelData());
        }

        public void InitializeBubbleGraph(string levelData)
        {

        }
        
        public void AddNode(BubbleNodeModel nodeModel)
        {
            _bubbles.Add(nodeModel.Coordinate, nodeModel);
        }
    }
}
