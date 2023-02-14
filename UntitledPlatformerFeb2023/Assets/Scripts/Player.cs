using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Upf23 {
  [RequireComponent(typeof(Rigidbody2D))]
  public class Player: MonoBehaviour {
    [SerializeField] GameObject _analogStickInputIndicator;
    [SerializeField] GameObject _directionIndicator;

    [SerializeField] float _walkSpeed = 3.5f;
    [SerializeField] float _runSpeed = 7f;
    [SerializeField] float _crawlSpeed = 2.75f;
    [SerializeField] float _jumpForce = 400f;

    [SerializeField] float _downInputThresholdAngle = -67.5f;

    [SerializeField] bool _useVectorMagnitudeForCrawlInput = false;
    [SerializeField] float _crawlInputThresholdAngle = -22.5f;
    [SerializeField] float _crawlMoveThreshold = 0.2f;

    [SerializeField] bool _useVectorMagnitudeForStraightInput = true;
    [SerializeField] float _straightWalkThreshold = 0.5f;
    [SerializeField] float _straightRunThreshold = 0.95f;

    [SerializeField] bool _useVectorMagnitudeForDiagonalInput = false;
    [SerializeField] float _diagonalInputThresholdAngle = 22.5f;
    [SerializeField] float _diagonalWalkThreshold = 0.2f;
    [SerializeField] float _diagonalRunThreshold = 0.55f;

    [SerializeField] float _upInputThresholdAngle = 80f;

    Rigidbody2D _rigidbody2D;

    Vector2 _moveInput = Vector2.zero;

    float _direction = 0f;
    float _moveSpeed = 0f;

    void Awake() {
      _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update() {
      _analogStickInputIndicator.transform.localPosition = _moveInput;
      _directionIndicator.transform.rotation = Quaternion.Euler(0f, 0f, _direction);
    }

    void FixedUpdate() {
      _updateDirectionAndMoveSpeed();
      Vector2 velocity = _rigidbody2D.velocity;
      velocity.x = _moveSpeed;
      _rigidbody2D.velocity = velocity;
    }

    void _updateDirectionAndMoveSpeed() {
      float minWalkThreshold = Mathf.Min(_straightWalkThreshold, _diagonalWalkThreshold);
      if (_moveInput.magnitude < minWalkThreshold) {
        _moveSpeed = 0f;
        return;
      }
      _updateDirection();
      _updateMoveSpeed();
    }

    void _updateDirection() {
      float distanceFromUpThresholdTo90 = 90f - _upInputThresholdAngle;
      float upInputMaxAngle = 90f + distanceFromUpThresholdTo90;
      float leftInputMinAngle = 180f - _diagonalInputThresholdAngle;
      float leftInputMaxNegativeAngle = -180f - _crawlInputThresholdAngle;
      float distanceFromDownThresholdToNegative90 = -90f - _downInputThresholdAngle;
      float downInputMinNegativeAngle = -90f + distanceFromDownThresholdToNegative90;

      float inputAngle = Utils.SignedAngle(_moveInput);

      if (inputAngle >= 0f) {
        if (inputAngle > 180f) {
          Debug.LogWarning("Updating player direction using invalid angle: " + inputAngle);
          _direction = 0f;
        }
        else if (inputAngle < _diagonalInputThresholdAngle) {
          _direction = 0f;
        }
        else if (inputAngle < _upInputThresholdAngle) {
          _direction = 45f;
        }
        else if (inputAngle < upInputMaxAngle) {
          _direction = 90f;
        }
        else if (inputAngle < leftInputMinAngle) {
          _direction = 135f;
        }
        else {
          _direction = -180f;
        }
      }
      else if (inputAngle >= -180f) {
        if (inputAngle < leftInputMaxNegativeAngle) {
          _direction = -180f;
        }
        else if (inputAngle < downInputMinNegativeAngle) {
          _direction = -135f;
        }
        else if (inputAngle < _downInputThresholdAngle) {
          _direction = -90f;
        }
        else if (inputAngle < _crawlInputThresholdAngle) {
          _direction = -45f;
        }
        else {
          _direction = 0f;
        }
      }
      else {
        Debug.LogWarning("Updating player direction using invalid angle: " + inputAngle);
        _direction = 0f;
      }
    }

    void _updateMoveSpeed() {
      float moveXMagnitude = Mathf.Abs(_moveInput.x);
      float moveVectorMagnitude = _moveInput.magnitude;

      _moveSpeed = 0f;

      if (_direction == 0f || _direction == 180f || _direction == -180f) {
        float moveMagnitude = _useVectorMagnitudeForStraightInput ? moveVectorMagnitude : moveXMagnitude;
        float sign = _direction == 0f ? 1 : -1;
        if (moveMagnitude >= _straightRunThreshold) {
          _moveSpeed = sign * _runSpeed;
        }
        else if (moveMagnitude >= _straightWalkThreshold) {
          _moveSpeed = sign * _walkSpeed;
        }
      }
      else if (_direction == 45f || _direction == 135f) {
        float moveMagnitude = _useVectorMagnitudeForDiagonalInput ? moveVectorMagnitude : moveXMagnitude;
        float sign = _direction == 45f ? 1 : -1;
        if (moveMagnitude >= _diagonalRunThreshold) {
          _moveSpeed = sign * _runSpeed;
        }
        else if (moveMagnitude >= _diagonalWalkThreshold) {
          _moveSpeed = sign * _walkSpeed;
        }
      }
      else if (_direction == -45f || _direction == -135f) {
        float moveMagnitude = _useVectorMagnitudeForCrawlInput ? moveVectorMagnitude : moveXMagnitude;
        float sign = _direction == -45f ? 1 : -1;
        if (moveMagnitude >= _crawlMoveThreshold) {
          _moveSpeed = sign * _crawlSpeed;
        }
      }
    }

    void OnJump(InputValue value) => _rigidbody2D.AddForce(new Vector2(0, _jumpForce));
    void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();
  }
}
