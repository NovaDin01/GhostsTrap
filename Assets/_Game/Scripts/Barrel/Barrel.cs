using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Barrel : MonoBehaviour, IObjectAttracted
{
    [SerializeField] private TypeAward typeAward;
    [SerializeField] private int minAwardValue = 1;
    [SerializeField] private int maxAwardValue = 5;

    private int awardValue;
    private bool isCaught;

    private bool despawnNotified;

    public int AwardValue => awardValue;
    public TypeAward AwardType => typeAward;

    public event Action<Barrel> OnCollectedBarrel;

    public event Action<Barrel> OnDespawned;
    
    public Transform SpawnPoint { get; private set; }

    public void SetSpawnPoint(Transform point)
    {
        SpawnPoint = point;
    }

    private void OnEnable()
    {
        isCaught = false;
        despawnNotified = false;

        awardValue = Random.Range(minAwardValue, maxAwardValue + 1);
    }

    public virtual CatchResult TryCatch(Transform catcher)
    {
        if (isCaught) return CatchResult.AlreadyCaught;

        transform.SetParent(catcher);
        transform.position = catcher.position;

        isCaught = true;
        return CatchResult.Caught;
    }

    public void OnCollected()
    {
        if (despawnNotified) return;

        OnCollectedBarrel?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NotifyDespawnOnce();
    }

    private void NotifyDespawnOnce()
    {
        if (despawnNotified) return;
        despawnNotified = true;
        OnDespawned?.Invoke(this);
    }
}