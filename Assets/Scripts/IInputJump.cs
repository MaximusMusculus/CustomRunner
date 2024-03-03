using UnityEngine;
using VContainer.Unity;

public interface IInput
{
    bool GetIsJump();
    Vector2 GetAxis();
}


public class Input : IInput, ITickable
{
    private bool _isJump;

    public bool GetIsJump()
    {
        return _isJump;
    }

    public Vector2 GetAxis()
    {
        return Vector2.right;
    }

    public void Tick()
    {
        _isJump = UnityEngine.Input.GetKey(KeyCode.Space) || UnityEngine.Input.GetMouseButtonDown(0);
    }
}


public class InputFotTest : IInput
{
    public bool GetIsJump()
    {
        return UnityEngine.Input.GetKey(KeyCode.Space) || UnityEngine.Input.GetMouseButtonDown(0);
    }

    public Vector2 GetAxis()
    {
        return new Vector2(UnityEngine.Input.GetAxis("Horizontal"), 0); // Input.GetAxis("Vertical"));
    }
}