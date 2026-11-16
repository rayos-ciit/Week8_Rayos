using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Serialized Fields
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _gravity = 28f;
    [SerializeField] private float _maxFallSpeed = 20f;

    [Header("Dependencies")]
    [SerializeField] private PlayerInput _playerInput;
    #endregion

    #region Private Variables
    private float _verticalVelocity;
    private bool _isGrounded;
    private bool _jumpBuffered;
    #endregion

    #region Properties
    public bool IsGrounded => _isGrounded;
    public float VerticalVelocity => _verticalVelocity;
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        BufferJumpInput();
    }

    private void FixedUpdate()
    {
        ProcessHorizontalMovement();
        ProcessVerticalMovement();

        _isGrounded = false;
    }
    #endregion

    #region Input Buffer
    private void BufferJumpInput()
    {
        if (_playerInput == null) return;

        if (_playerInput.JumpRequested)
        {
            _jumpBuffered = true;
            _playerInput.ConsumeJump();
        }
    }
    #endregion

    #region Movement Processing
    private void ProcessHorizontalMovement()
    {
        if (_playerInput == null) return;

        float horizontalInput = _playerInput.HorizontalInput;
        Vector3 horizontalDelta = new Vector3(horizontalInput * _moveSpeed * Time.fixedDeltaTime, 0f, 0f);

        transform.position += horizontalDelta;
    }

    private void ProcessVerticalMovement()
    {
        ApplyJump();
        ApplyGravity();

        Vector3 verticalDelta = new Vector3(0f, _verticalVelocity * Time.fixedDeltaTime, 0f);
        transform.position += verticalDelta;
    }

    private void ApplyJump()
    {
        if (!_isGrounded || !_jumpBuffered) return;

        _verticalVelocity = _jumpForce;
        _isGrounded = false;
        _jumpBuffered = false;
    }

    private void ApplyGravity()
    {
        _verticalVelocity -= _gravity * Time.fixedDeltaTime;

        if (_verticalVelocity < -_maxFallSpeed)
        {
            _verticalVelocity = -_maxFallSpeed;
        }
    }
    #endregion

    #region State Alteration Methods (Called by Collision Handler)
    public void SetGrounded(bool grounded)
    {
        _isGrounded = grounded;

        if (_isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = 0f;
        }
    }

    public void ZeroVerticalVelocity()
    {
        _verticalVelocity = 0f;
    }
    #endregion
}
