using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slea {
  [RequireComponent(typeof(Controller2D))]
  public class Player: MonoBehaviour {
    Controller2D _controller;

    void Awake() {
      _controller = GetComponent<Controller2D>();
    }
  }
}
