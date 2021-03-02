using System;
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

        public void Showlines(Vector2[] points)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = points.Length;
            for (var i = 0; i < points.Length; i++)
            {
                var vector3Point = points[i];
                _lineRenderer.SetPosition(i, vector3Point);
            }

            _showLine = true;
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

        public void HideLineRender()
        {
            _lineRenderer.positionCount = 0;
            _lineRenderer.enabled = false;
        }
    }
}
