using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slea {
  [RequireComponent(typeof(Controller2D))]
  public class Player: MonoBehaviour {
    Controller2D _controller;

    float _moveSpeed = 6f;
    float gravity = -20f;
    Vector3 _velocity;

    void Awake() {
      _controller = GetComponent<Controller2D>();
    }

    void Update() {
      Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

      _velocity.x = input.x * _moveSpeed;
      _velocity.y += gravity * Time.deltaTime;
      _controller.Move(_velocity * Time.deltaTime);
    }
  }
}
