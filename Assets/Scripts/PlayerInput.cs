using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Properties
    public float HorizontalInput { get; private set; }
    public bool JumpRequested { get; private set; }
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        ReadMovementInput();
        ReadJumpInput();
    }
    #endregion

    #region Input Reading Helpers
    private void ReadMovementInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void ReadJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            JumpRequested = true;
            return;
        }

        JumpRequested = false;
    }
    #endregion

    #region Public State Reset
    public void ConsumeJump()
    {
        JumpRequested = false;
    }
    #endregion
}
