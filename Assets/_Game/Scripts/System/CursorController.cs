using UnityEngine;
//Класс изменения внешнего вида курсора (когда ставить ловушку 1 спрайт, когда поставил 2)
public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D _trapCursor; //курсор когда нужно бросить ловушку
    [SerializeField] private Texture2D _defaultCursor; //курсор когда ловушки расставлены
    private void Start()
    {
        SetTrapCursor();
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