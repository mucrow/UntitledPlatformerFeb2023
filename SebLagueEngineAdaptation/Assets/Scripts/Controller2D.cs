using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slea {
  [RequireComponent(typeof(BoxCollider2D))]
  public class Controller2D: MonoBehaviour {
    [SerializeField] LayerMask _collisionMask;
    [SerializeField] int _horizontalRayCount = 4;
    [SerializeField] int _verticalRayCount = 4;

    float _horizontalRaySpacing;
    float _verticalRaySpacing;

    const float _skinWidth = 0.015f;

    BoxCollider2D _boxCollider;
    RaycastOrigins _raycastOrigins;

    void Awake() {
      _boxCollider = GetComponent<BoxCollider2D>();
      _calculateRaySpacing();
    }

    // TODO rename parameter position delta ? this is not velocity, at least not per second. it is multiplied by Time.deltaTime in Player#Update where it is called.
    public void Move(Vector3 velocity) {
      _updateRaycastOrigins();

      if (velocity.x != 0) {
        _horizontalCollisions(ref velocity);
      }
      if (velocity.y != 0) {
        _verticalCollisions(ref velocity);
      }

      transform.Translate(velocity);
    }

    void _horizontalCollisions(ref Vector3 velocity) {
      float directionX = Mathf.Sign(velocity.x);
      float rayLength = Mathf.Abs(velocity.x) + _skinWidth;

      for (int i = 0; i < _horizontalRayCount; ++i) {
        Vector2 rayOrigin = directionX < 0 ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight;
        // why did we delete `+ _velocity.x` ? why does it exist in vertical collisions ?
        rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, _collisionMask);

        Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

        if (hit) {
          // set our X velocity to only the distance needed to collide
          velocity.x = (hit.distance - _skinWidth) * directionX;
          // shortens the ray length so subsequent vertical rays do not hit farther than an earlier one
          rayLength = hit.distance;
        }
      }
    }

    void _verticalCollisions(ref Vector3 velocity) {
      float directionY = Mathf.Sign(velocity.y);
      float rayLength = Mathf.Abs(velocity.y) + _skinWidth;

      for (int i = 0; i < _verticalRayCount; ++i) {
        Vector2 rayOrigin = directionY < 0 ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
        rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, _collisionMask);

        Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

        if (hit) {
          // set our Y velocity to only the distance needed to collide
          velocity.y = (hit.distance - _skinWidth) * directionY;
          // shortens the ray length so subsequent vertical rays do not hit farther than an earlier one
          rayLength = hit.distance;
        }
      }
    }

    void _updateRaycastOrigins() {
      Bounds bounds = _boxCollider.bounds;
      bounds.Expand(-2f * _skinWidth);
      _raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
      _raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
      _raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
      _raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void _calculateRaySpacing() {
      Bounds bounds = _boxCollider.bounds;
      bounds.Expand(-2f * _skinWidth);

      _horizontalRayCount = Mathf.Max(_horizontalRayCount, 2);
      _verticalRayCount = Mathf.Max(_verticalRayCount, 2);

      _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
      _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
    }

    struct RaycastOrigins {
      public Vector2 TopLeft;
      public Vector2 TopRight;
      public Vector2 BottomLeft;
      public Vector2 BottomRight;
    }
  }
}
