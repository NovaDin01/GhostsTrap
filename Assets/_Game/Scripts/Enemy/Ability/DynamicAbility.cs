using UnityEngine;

public class DynamicAbility : IGhostAbility
{
    private Ghost _ghost;
    
    private float _minMult;
    private float _maxMult;

    private float minTime;
    private float maxTime;
    
    public void Init(Ghost ghost)
    {
        _ghost = ghost;
    }

    public void Tick()
    {
        if (_ghost.Movement is ISpeedModifiable speedModifiable)
        {
            float multiplier = Random.Range(_minMult, _maxMult);
            speedModifiable.SetSpeedMultiplier(multiplier);
        }
    }
}