using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Upf23 {
  [RequireComponent(typeof(Rigidbody2D))]
  public class Player: MonoBehaviour {
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _jumpForce = 400f;

    Rigidbody2D _rigidbody2D;

    float _moveInput = 0;

    void Awake() {
      _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
      Vector2 velocity = _rigidbody2D.velocity;
      velocity.x = _moveInput * _moveSpeed;
      _rigidbody2D.velocity = velocity;
    }

    public void OnJump(InputValue value) => _rigidbody2D.AddForce(new Vector2(0, _jumpForce));
    public void OnMove(InputValue value) => _moveInput = value.Get<Vector2>().x;
  }
}
