using System;
using UnityEngine;

public class GhostFactory : MonoBehaviour
{
    [SerializeField] private Ghost ghostPrefab;

    private void Start()
    {
        SpawnGhost();
    }

    public void SpawnGhost()
    {
        Ghost ghost = Instantiate(ghostPrefab);
        IGhostMovement movement = new BaseMove();

        ghost.Spawn(movement);
        
    }
}