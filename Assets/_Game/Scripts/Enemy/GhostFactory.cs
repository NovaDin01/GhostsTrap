using UnityEngine;

public class GhostFactory : MonoBehaviour
{
    [SerializeField] private Ghost ghostPrefab;

    public void SpawnGhost()
    {
        Ghost ghost = Instantiate(ghostPrefab);
        IGhostMovement movement = new BaseMove();
    }
}