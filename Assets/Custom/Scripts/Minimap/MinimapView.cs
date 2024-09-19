using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TD.Extensions;
using UnityEngine;

public class MinimapView : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraBounds _cameraBounds;
    [SerializeField] private RectTransform _mapTransform;
    [SerializeField] private RectTransform _maskBounds;
    [SerializeField] private MMSimpleObjectPooler _objectPool;
    [SerializeField] private RectTransform _userView;
    [SerializeField] private FrameSkipper _frameSkipper;
    [Range(0, 0.9f)] [SerializeField] float _forwardOffsetPercent = 0.2f;
    [SerializeField] float _clampingOffset = 5f;

    protected Dictionary<Transform, RectTransform> _activePlayers;
    protected Transform _userTransform;
    protected Character _character;
    protected Vector2 _maskBoundsSize;
    protected Vector2 _mapCenter;
    protected Vector2 _mapSize;
    private float _lastAngle;

    private void Awake()
    {
        _activePlayers = new Dictionary<Transform, RectTransform>();
        _mapSize = _mapTransform.rect.size;
        _maskBoundsSize = _maskBounds.rect.size;
        _mapCenter = _mapSize / 2;
    }

    public void Update()
    {
        if (_frameSkipper.FrameSkipped)
        {
            return;
        }

        foreach (var keyValuePair in _activePlayers)
        {
            if (keyValuePair.Key != null)
            {
                UpdatePlayerView(keyValuePair.Key, keyValuePair.Value);
            }
        }
    }

    public void ShowPlayer(Transform player, bool isUser, Character character)
    {
        if (isUser)
        {
            _userTransform = player;
            _character = character;
            Debug.Log(character);
        }

        GameObject playerViewGo = GetViewForPlayer(isUser);

        if (!(playerViewGo.transform is RectTransform rectTransform))
        {
            Debug.LogError("Pool prefab is not rt", playerViewGo);
            return;
        }

        _activePlayers.Add(player, rectTransform);
        rectTransform.SetParent(_mapTransform, false);
        rectTransform.SetSiblingIndex(isUser ? 1 : 0);
        UpdatePlayerView(player, rectTransform);
        playerViewGo.SetActive(true);
    }

    private GameObject GetViewForPlayer(bool isUser)
    {
        if (isUser)
        {
            return _userView.gameObject;
        }

        return _objectPool.GetPooledGameObject();
    }

    private void UpdatePlayerView(Transform player, RectTransform view)
    {
        Vector2 positionPercent = _cameraBounds.GetRelativePositionPercent(player.position);
        Vector2 viewPosition = positionPercent.MultiplyAxiswise(_mapSize);

        if (player == _userTransform)
        {
            view.anchoredPosition = viewPosition;
            RotateCamera(viewPosition);
            return;
        }

        if (_userView == null)
        {
            view.anchoredPosition = viewPosition;
            return;
        }

        Vector2 forwardLookOffset = -GetForwardLookOffset().RotateAroundPivot(Vector2.zero, -_lastAngle);
        Vector2 mapCenter = _userView.anchoredPosition + forwardLookOffset;
        Vector2 clampedPosition = viewPosition.ClampInRotatedRectangle(
            mapCenter,
            _maskBoundsSize.x - _clampingOffset,
            _maskBoundsSize.y - _clampingOffset,
            -_lastAngle);
        view.anchoredPosition = clampedPosition;
    }

    private void RotateCamera(Vector2 playerMapPosition)
    {
        float targetAngle = -Vector2.SignedAngle(Vector2.right, _character.CameraDirection.GetXZ()) + 90f;
        Vector2 rotatedPoint = playerMapPosition.RotateAroundPivot(_mapCenter, targetAngle);

        Vector2 rotationOffset = -(rotatedPoint - playerMapPosition);
        Vector2 forwardLookOffset = GetForwardLookOffset();
        Vector2 mapPosition = _mapCenter + rotationOffset + forwardLookOffset - playerMapPosition;
        _lastAngle = targetAngle;

        _mapTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
        _mapTransform.anchoredPosition = mapPosition;
    }

    private Vector2 GetForwardLookOffset()
    {
        return new Vector2(0, -_maskBoundsSize.y / 2 * _forwardOffsetPercent);
    }

    public void HideView(Transform player)
    {
        if (!_activePlayers.ContainsKey(player))
        {
            Debug.LogError("Trying to remove not existing view", player);
            return;
        }

        RectTransform view = _activePlayers[player];
        view.gameObject.SetActive(false);
        _activePlayers.Remove(player);
    }
}