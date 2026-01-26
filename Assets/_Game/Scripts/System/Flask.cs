using UnityEngine;

public class Flask : MonoBehaviour
{
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private bool destroyOnBreak = true;

    private bool isBroken;

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, transform.rotation);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }

        if (destroyOnBreak)
        {
            Destroy(gameObject);
        }
    }
}
