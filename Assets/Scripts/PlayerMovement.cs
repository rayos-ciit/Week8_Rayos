using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Serialized Fields
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _gravity = 28f;

    [Header("Dependencies")]
    [SerializeField] private PlayerInput _playerInput;
    #endregion

    #region Private Variables
    private float _verticalVelocity;
    private bool _isGrounded;
    #endregion

    #region Properties
    public bool IsGrounded => _isGrounded;
    public float VerticalVelocity => _verticalVelocity;
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        ProcessHorizontalMovement();
        ProcessVerticalMovement();
    }
    #endregion

    #region Movement Processing
    private void ProcessHorizontalMovement()
    {
        if (_playerInput == null) return;

        float horizontalInput = _playerInput.HorizontalInput;
        Vector3 horizontalDelta = new Vector3(horizontalInput * _moveSpeed * Time.deltaTime, 0f, 0f);

        transform.position += horizontalDelta;
    }

    private void ProcessVerticalMovement()
    {
        ApplyJump();
        ApplyGravity();

        Vector3 verticalDelta = new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f);
        transform.position += verticalDelta;

        //reset grounded check before collision checks
        _isGrounded = false;
    }

    private void ApplyJump()
    {
        //jump only if grounded and if input has been confirmed
        if (!_isGrounded) return;
        if (_playerInput == null || !_playerInput.JumpRequested) return;

        _verticalVelocity = _jumpForce;
        _isGrounded = false;
        _playerInput.ConsumeJump();
    }

    private void ApplyGravity()
    {
        if (_isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = 0f;
            return;
        }

        _verticalVelocity -= _gravity * Time.deltaTime;
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
