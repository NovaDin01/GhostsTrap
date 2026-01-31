using UnityEngine;
public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D _trapCursor; //ęóđńîđ ęîăäŕ íóćíî áđîńčňü ëîâóřęó
    [SerializeField] private Texture2D _defaultCursor; //ęóđńîđ ęîăäŕ ëîâóřęč đŕńńňŕâëĺíű
    private void Start()
    {
        SetTrapCursor();
        Cursor.SetCursor(_defaultCursor, new Vector2(_defaultCursor.width / 2, _defaultCursor.height / 2), CursorMode.Auto);
    }

    public void SetTrapCursor()
    {
        Cursor.SetCursor(_trapCursor,new Vector2(_trapCursor.width/2, _trapCursor.height/2), CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(_defaultCursor, new Vector2(_trapCursor.width / 2, _trapCursor.height / 2), CursorMode.Auto);
    }
}