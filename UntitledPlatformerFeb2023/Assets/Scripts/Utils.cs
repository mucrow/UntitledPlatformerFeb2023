using UnityEngine;

namespace Upf23 {
  public class Utils {
    public static float SignedAngle(Vector2 v) {
      return Vector2.SignedAngle(Vector2.right, v);
    }
  }
}
