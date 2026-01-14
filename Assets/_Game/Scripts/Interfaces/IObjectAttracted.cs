using UnityEngine;

public enum CatchResult
{
    AlreadyCaught,// уже пойман
    Resisted,     // попытка была, но не пойман
    Caught        // пойман 
}

public enum TypeAward
{
    Money,
    Hp,
    Ability
}

public interface IObjectAttracted
{
    CatchResult TryCatch(Transform catcher);
    TypeAward AwardType { get; }
    int AwardValue { get; }
    
    void OnCollected();
}

