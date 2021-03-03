using UnityEngine;

namespace Assets.Code.ShootEffect
{
    public class MouseShootView : MonoBehaviour
    {
        private bool _showLine = false;
        private LineRenderer _lineRenderer;
        
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
        }

        public void Tick(Vector2 [] updatedPositions)
        {
            if(_showLine == false) return;
            for (var i = 0; i < updatedPositions.Length; i++)
            {
                Vector3 updatedPosition = updatedPositions[i];
                _lineRenderer.SetPosition(i, updatedPosition);
            }
        }

        public void TrySetPointsCount(int dotsCount)
        {
            if (_lineRenderer.enabled == false)
            {
                _lineRenderer.enabled = true;
            }
              
            if(dotsCount == _lineRenderer.positionCount) return;
            while (dotsCount != _lineRenderer.positionCount)
            {
                if (dotsCount > _lineRenderer.positionCount)
                {
                    _lineRenderer.positionCount++;
                }
                else
                {
                    _lineRenderer.positionCount--;
                }
            }
        }

        public void SetPosition(int i, Vector2 dot)
        {
            _lineRenderer.SetPosition(i, dot);
        }

        public void Clear()
        {
            _lineRenderer.positionCount = 0;
            _lineRenderer.enabled = false;
        }
    }
}
