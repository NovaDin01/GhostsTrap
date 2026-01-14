using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Barrel : MonoBehaviour, IObjectAttracted
{
    [SerializeField] private TypeAward typeAward;
    [SerializeField] private int minAwardValue;
    [SerializeField] private int maxAwardValue;
    
    protected int awardValue;
    protected bool _isCaught;    
    
    public int AwardValue => awardValue;
    public TypeAward AwardType => typeAward;
    
    public event Action<Barrel> OnCollectedBarrel;


    private void Awake()
    {
        awardValue = Random.Range(minAwardValue, maxAwardValue);
    }

    public virtual CatchResult TryCatch(Transform catcher)
    {
        if (_isCaught) return CatchResult.AlreadyCaught;
        
        AttachToCatcher(catcher);
        _isCaught = true;
        return CatchResult.Caught;
    }
    
    protected void AttachToCatcher(Transform catcher)
    {
        transform.SetParent(catcher);
        transform.position = catcher.position;
    }

    public void OnCollected()
    {
        OnCollectedBarrel?.Invoke(this);
        Destroy(gameObject);
    }
    
    
}
