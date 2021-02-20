using System.Collections.Generic;
using Assets.Code.LevelGeneration;
using Assets.Code.Utils;
using Zenject;

namespace Assets.Code.Bubble
{
    public class BubbleGraph
    {
        private readonly LevelDataContext _levelDataContext;
        private Dictionary<Coordinate, IBubbleNodeController> _bubbles;
        
        [Inject(Id = Constants.InitialBubbleFactory)] private BubbleFactory _bubbleFactory;
        
        public BubbleGraph(LevelDataContext levelDataContext)
        {
            _levelDataContext = levelDataContext;
            _bubbles = new Dictionary<Coordinate, IBubbleNodeController>();
        }

        private BubbleType ColorToTypeConversion(string color)
        {
            if (color == "r") return BubbleType.Red;
            else if (color == "g") return BubbleType.Green;
            else if (color == "b") return BubbleType.Blue;
            else if (color == "e") return BubbleType.Empty;
            return BubbleType.Blue;
        }

        public void InitializeBubbleGraph()
        {
            string levelData = _levelDataContext.GetSelectedLevelData();
            var lines = levelData.Split('\n');
            for (var row = 0; row < lines.Length; row++)
            {
                var text = lines[row].Trim();
        
                var colors = text.Split(',');
        
                for (var col = 0; col < colors.Length; col++)
                {
                    var color = colors[col];
                    var bubbleType = ColorToTypeConversion(color);
                    
                    if(bubbleType == BubbleType.Empty) continue;
                    
                    var node = _bubbleFactory.Create(bubbleType, new Coordinate() { Row = row, Col = col });
                    AddNode(node);
                    if (col > 0)
                    {
                        var leftIndex = col - 1;
                        _bubbles.TryGetValue(new Coordinate() {Row = row, Col = leftIndex}, out var leftNode);
                        if (leftNode != null)
                        {
                            node.Left = _bubbles[new Coordinate() { Row = row, Col = leftIndex }];
                            node.Left.Right = node;
                        }
                    }
        
                    if (row > 0)
                    {
                        if (col > 0)
                        {
                            _bubbles.TryGetValue(new Coordinate { Col = col - 1, Row = row }, out var upperLeftNode);
                            if (upperLeftNode != null)
                            {
                                node.TopLeft = upperLeftNode;
                                upperLeftNode.BottomRight = node;
                            }
                        }
        
                        _bubbles.TryGetValue(new Coordinate() { Col = col, Row = row - 1 }, out var upperNode);
        
                        if (upperNode != null)
                        {
                            node.TopRight = upperNode;
                            upperNode.BottomLeft = node;
                        }
                    }
                }
            }
        }

        public void AddNode(IBubbleNodeController bubbleNodeController)
        {
            _bubbles.Add(bubbleNodeController.Coordinate, bubbleNodeController);
        }
    }
}
