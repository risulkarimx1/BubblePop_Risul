using System;
using System.Collections.Generic;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.ShootEffect
{
    public class MouseShootEffect : IInitializable, ITickable, IDisposable
    {
        private Camera _mainCamera;

        private List<Vector2> _dots;
        private List<GameObject> _dotObjects;

        public MouseShootEffect(CameraEffects cameraEffects)
        {
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

        public void Initialize()
        {
        }

        public void Tick()
        {
            if (Input.GetMouseButton(0))
            {
                if (_dots == null) return;
                _dots.Clear();
                var touch = Input.mousePosition;
                touch.z = Mathf.Abs(0.0f - _mainCamera.transform.position.z);
                touch = _mainCamera.ScreenToWorldPoint(touch);
                var direction = (Vector2) touch - Constants.FirstPosition;

                RaycastHit2D hit = Physics2D.Raycast(Constants.FirstPosition, direction);

                if (hit.collider == null) return;

                _dots.Add(Constants.FirstPosition);

                if (hit.collider.CompareTag(Constants.SideWallTag))
                {
                    PerformReycastOnWall(hit, direction);
                }
                else
                {
                    _dots.Add(hit.point);
                    DrawPaths();
                }
            }

            else if (Input.GetMouseButtonUp(0))
            {
                if (_dots.Count < 2)
                {
                    return;
                }

                _dots.Clear();
                _dotObjects.ForEach(g => g.SetActive(false));
            }
        }


        void PerformReycastOnWall(RaycastHit2D previousHit, Vector2 directionIn)
        {
            _dots.Add(previousHit.point);
            var normal = Mathf.Atan2(previousHit.normal.y, previousHit.normal.x);
            var newDirection = normal + (normal - Mathf.Atan2(directionIn.y, directionIn.x));
            var reflection = new Vector2(-Mathf.Cos(newDirection), -Mathf.Sin(newDirection));
            var newCastPoint = previousHit.point + (2 * reflection);
            var hit2 = Physics2D.Raycast(newCastPoint, reflection);

            if (hit2.collider != null)
            {
                if (hit2.collider.CompareTag(Constants.SideWallTag))
                {
                    PerformReycastOnWall(hit2, reflection);
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

            for (var i = 0; i < _dots.Count; i++)
            {
                _dotObjects[i].SetActive(true);
                _dotObjects[i].transform.position = _dots[i];
            }
        }

        public void Dispose()
        {
        }
    }
}