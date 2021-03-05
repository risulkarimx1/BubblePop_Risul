using DG.Tweening;
using UniRx.Async;
using UnityEngine;

public class CameraEffectsController : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private Material _rippleMaterial;
    [SerializeField] private float _maxAmount = 50f;

    [Range(0, 1)] [SerializeField] private float _friction = .9f;

    [SerializeField] private float _amount = 0f;
    private static readonly int _centerX = Shader.PropertyToID("_CenterX");
    private static readonly int _centerY = Shader.PropertyToID("_CenterY");
    private static readonly int _amountProperty = Shader.PropertyToID("_Amount");

    private Transform _transform;
    private readonly Vector3 _shakePosition = (Vector3.one - Vector3.back) * .25f;
    private Vector3 _defaultPosition;

    public Camera MainCamera => _camera;
    private bool _isShaking = false;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _transform = GetComponent<Transform>();
        _defaultPosition = _transform.position;
    }

    public void ShowRipple(Vector2 position)
    {
        _ = Ripple(position);
    }

    public void ShakeCamera()
    {
        if (_isShaking) return;

        _isShaking = true;
        DOTween.Sequence()
            .Append(_transform.DOShakePosition(1f, _shakePosition))
            .Append(_transform.DOMove(_defaultPosition, 0.25f))
            .AppendCallback(() => _isShaking = false);
    }

    private async UniTask Ripple(Vector2 pos)
    {
        _amount = _maxAmount;
        _rippleMaterial.SetFloat(_centerX, pos.x);
        _rippleMaterial.SetFloat(_centerY, pos.y);

        var startingTime = Time.time;
        while (Time.time - startingTime < 5)
        {
            _rippleMaterial.SetFloat(_amountProperty, _amount);
            _amount *= _friction;
            await UniTask.Yield();
        }

        _amount = 0;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, _rippleMaterial);
    }
}