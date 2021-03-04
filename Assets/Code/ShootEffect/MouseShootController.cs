using System;
using System.Collections.Generic;
using Assets.Code.Managers;
using Assets.Code.Signals;
using Assets.Code.Utils;
using UnityEngine;
using Zenject;

namespace Assets.Code.ShootEffect
{
    public class MouseShootController : ITickable, IDisposable
    {
        private readonly MouseShootView _mouseShootView;
        private readonly SignalBus _signalBus;
        private readonly GameStateController _gameStateController;
        private Camera _mainCamera;

        private List<Vector2> _collisions = new List<Vector2>();
        private Vector2 _shootDirection;

        private bool _isWaitingToShoot;
        
        public MouseShootController(CameraEffects cameraEffects, MouseShootView mouseShootView, SignalBus signalBus, GameStateController gameStateController)
        {
            _mouseShootView = mouseShootView;
            _signalBus = signalBus;
            _gameStateController = gameStateController;
            _mainCamera = cameraEffects.MainCamera;
            _isWaitingToShoot = false;
            _signalBus.Subscribe<GameStateChangeSignal>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangeSignal gameStateChangeSignal)
        {
            _isWaitingToShoot = gameStateChangeSignal.State == GameState.WaitingToShoot;
        }

        public void Tick()
        {
            if(_isWaitingToShoot == false) return;
            if(_gameStateController.CurrentSate != GameState.WaitingToShoot) return;
             
            // on clicked
            if (Input.GetMouseButton(0))
            {
                if (_collisions == null) return;
                _collisions.Clear();
                var touch = Input.mousePosition;
                touch.z = Mathf.Abs(0.0f - _mainCamera.transform.position.z);
                touch = _mainCamera.ScreenToWorldPoint(touch);
                if (touch.y < Constants.FirstPosition.y + 1)
                {
                    touch.y = Constants.FirstPosition.y + 1;
                }

                var direction = (Vector2) touch - Constants.FirstPosition;

                _shootDirection = direction;

                var hit = Physics2D.Raycast(Constants.FirstPosition, direction, 1000, Constants.InputEffectsMask);

                if (hit.collider == null) return;

                _collisions.Add(Constants.FirstPosition);

                if (hit.collider.CompareTag(Constants.SideWallTag))
                {
                    PerformRayCastOnWall(hit, direction);
                }
                else
                {
                    _collisions.Add(hit.point);
                    DrawPaths();
                }
            }

            // on release
            else if (Input.GetMouseButtonUp(0))
            {
                if (_collisions.Count < 2)
                {
                    return;
                }

                _signalBus.Fire(new StrikeSignal() {Direction = _shootDirection});

                _mouseShootView.Clear();
                _collisions.Clear();
            }
        }


        void PerformRayCastOnWall(RaycastHit2D previousHit, Vector2 directionIn)
        {
            _collisions.Add(previousHit.point);
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
                    _collisions.Add(hit2.point);
                    DrawPaths();
                }
            }
            else
            {
                DrawPaths();
            }
        }
        
        private void DrawPaths()
        {
            _mouseShootView.TrySetPointsCount(_collisions.Count);

            for (var i = 0; i < _collisions.Count; i++)
            {
                _mouseShootView.SetPosition(i, _collisions[i]);
            }
        }

        public void Dispose()
        {
            
        }
    }
}