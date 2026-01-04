using UnityEngine;

public class BaseMove : IGhostMovement
{
    private Ghost _ghost;
    
    public void Init(Ghost ghost)
    {
        _ghost = ghost;
    }

    public void Tick()
    {
        _ghost.transform.position = Vector2.MoveTowards(_ghost.transform.position, Vector2.right, )
    }
}