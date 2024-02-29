using UnityEngine;

public interface IInputJump
{
    bool GetJump();
}

public interface IInputAxis
{
    Vector2 GetAxis();
}

public class InputJump : IInputJump
{
    public bool GetJump()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
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
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}