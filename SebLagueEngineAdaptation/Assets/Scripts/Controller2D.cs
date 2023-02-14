using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slea {
  [RequireComponent(typeof(BoxCollider2D))]
  public class Controller2D: MonoBehaviour {
    [SerializeField] int _horizontalRayCount = 4;
    [SerializeField] int _verticalRayCount = 4;

    float _horizontalRaySpacing;
    float _verticalRaySpacing;

    const float _skinWidth = 0.015f;

    BoxCollider2D _boxCollider;
    RaycastOrigins _raycastOrigins;

    void Awake() {
      _boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update() {
      _updateRaycastOrigins();
      _calculateRaySpacing();
      for (int i = 0; i < _verticalRayCount; ++i) {
        Debug.DrawRay(_raycastOrigins.BottomLeft + Vector2.right * _verticalRaySpacing * i, Vector2.up * -2, Color.red);
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
