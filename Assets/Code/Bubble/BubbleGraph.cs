using System.Collections.Generic;
using Assets.Code.LevelGeneration;

namespace Assets.Code.Bubble
{
    public class BubbleGraph
    {
        private readonly BubbleFactory _bubbleFactory;
        private readonly LevelDataContext _levelDataContext;
        private Dictionary<Coordinate, IBubbleNodeController> _bubbles;
        private Dictionary<int, IBubbleNodeController> _viewToControllerMap;
        
        public BubbleGraph(BubbleFactory bubbleFactory, LevelDataContext levelDataContext)
        {
            _bubbleFactory = bubbleFactory;
            _levelDataContext = levelDataContext;
            _bubbles = new Dictionary<Coordinate, IBubbleNodeController>();
            _viewToControllerMap = new Dictionary<int, IBubbleNodeController>();
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
                    var bubbleType = BubbleUtility.ConvertColorToBubbleType(color);
                    
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

        public void AddNode(IBubbleNodeController bubbleController)
        {
            _bubbles.Add(bubbleController.Coordinate, bubbleController);
            _viewToControllerMap.Add(bubbleController.BubbleNodeView.GetInstanceID(), bubbleController);
        }
    }
}
