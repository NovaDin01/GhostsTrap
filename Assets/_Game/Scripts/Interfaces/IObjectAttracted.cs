using UnityEngine;

public enum CatchResult
{
    AlreadyCaught,// уже пойман
    Resisted,     // попытка была, но не пойман
    Caught        // пойман 
}

public interface IObjectAttracted
{
    CatchResult TryCatch(Transform catcher);
    bool IsCaught();
}