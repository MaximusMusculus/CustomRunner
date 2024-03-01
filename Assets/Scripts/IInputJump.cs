using UnityEngine;
using VContainer.Unity;

public interface IInputJump
{
    bool GetIsJump();
}

public interface IInputAxis
{
    Vector2 GetAxis();
}

public class InputJump : IInputJump, ITickable
{
    private bool _isJump;
    public bool GetIsJump()
    {
        return _isJump;
    }
    public void Tick()
    {
        _isJump = Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(0);
    }
}
public class InputRunnerAxis : IInputAxis
{
    public Vector2 GetAxis()
    {
        return Vector2.right;
    }
}

public class InputAxisFotTest : IInputAxis
{
    public Vector2 GetAxis()
    {
        return new Vector2(Input.GetAxis("Horizontal"), 0); // Input.GetAxis("Vertical"));
    }
}