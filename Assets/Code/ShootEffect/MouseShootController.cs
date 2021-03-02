using System.Collections.Generic;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.ShootEffect
{
    public class MouseShootController : ITickable
    {
        private readonly MouseShootView _mouseShootView;
        private Camera _mainCamera;

        private List<Vector2> _dots;
        private List<GameObject> _dotObjects;

        public MouseShootController(CameraEffects cameraEffects, MouseShootView mouseShootView)
        {
            _mouseShootView = mouseShootView;
            _mainCamera = cameraEffects.MainCamera;
            _dots = new List<Vector2>();
            _dotObjects = new List<GameObject>();
            for (int i = 0; i < 20; i++)
            {
                var dotObject = CreateDot(Constants.HiddenPosition);
                _dotObjects.Add(dotObject);
                dotObject.SetActive(false);
            }
        }
        
        public void Tick()
        {
            // on clicked
            if (Input.GetMouseButton(0))
            {
                if (_dots == null) return;
                _dots.Clear();
                var touch = Input.mousePosition;
                touch.z = Mathf.Abs(0.0f - _mainCamera.transform.position.z);
                touch = _mainCamera.ScreenToWorldPoint(touch);
                if (touch.y < Constants.FirstPosition.y + 1)
                {
                    touch.y = Constants.FirstPosition.y + 1;
                }
                var direction = (Vector2) touch - Constants.FirstPosition;

                var hit = Physics2D.Raycast(Constants.FirstPosition, direction, 1000, Constants.InputEffectsMask);

                if (hit.collider == null) return;

                _dots.Add(Constants.FirstPosition);

                if (hit.collider.CompareTag(Constants.SideWallTag))
                {
                    PerformRayCastOnWall(hit, direction);
                }
                else
                {
                    _dots.Add(hit.point);
                    DrawPaths();
                }
            }

            // on release
            else if (Input.GetMouseButtonUp(0))
            {
                if (_dots.Count < 2)
                {
                    return;
                }

                _mouseShootView.Clear();
                _dots.Clear();
                _dotObjects.ForEach(g => g.SetActive(false));
            }
        }


        void PerformRayCastOnWall(RaycastHit2D previousHit, Vector2 directionIn)
        {
            _dots.Add(previousHit.point);
            var normal = Mathf.Atan2(previousHit.normal.y, previousHit.normal.x);
            var newDirection = normal + (normal - Mathf.Atan2(directionIn.y, directionIn.x));
            var reflection = new Vector2(-Mathf.Cos(newDirection), -Mathf.Sin(newDirection));
            var newCastPoint = previousHit.point + (2 * reflection);
            var hit2 = Physics2D.Raycast(newCastPoint, reflection, 1000, Constants.InputEffectsMask);

            if (hit2.collider != null)
            {
                if (hit2.collider.CompareTag(Constants.SideWallTag))
                {
                    PerformRayCastOnWall(hit2, reflection);
                }
                else
                {
                    _dots.Add(hit2.point);
                    DrawPaths();
                }
            }
            else
            {
                DrawPaths();
            }
        }

        private GameObject CreateDot(Vector2 firstPosition)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = firstPosition;
            return go;
        }

        private void DrawPaths()
        {
            foreach (var gameObject in _dotObjects)
            {
                gameObject.SetActive(false);
            }
            
            _mouseShootView.TrySetPointsCount(_dots.Count);
            
            for (var i = 0; i < _dots.Count; i++)
            {
                _mouseShootView.SetPosition(i, _dots[i]);
            }
        }
    }
}