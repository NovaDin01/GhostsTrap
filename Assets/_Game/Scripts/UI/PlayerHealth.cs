using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    
    [SerializeField] private List<Image> heartSprites;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite nullHeart;

    private void Start()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        Player.Instance.OnApplyDamage += MinusHeart;
        Player.Instance.OnApplyHeal += PlusHeart;

        int currentHp = Player.Instance.CurrentHp;
        int heartsToShow = Mathf.Min(currentHp, heartSprites.Count);

        foreach (var heart in heartSprites) heart.sprite = nullHeart;
        for (int i = 0; i < heartsToShow; i++) heartSprites[i].sprite = fullHeart;

    }

    private void PlusHeart()
    {
        for (int i = heartSprites.Count - 1; i >= 0; i--)
        {
            if (heartSprites[i].sprite == emptyHeart)
            {
                heartSprites[i].sprite = fullHeart;
                return;
            }
        }
    }

    public void PlusMaxHeart()
    {
        for (int i = 0; i < heartSprites.Count; i++)
        {
            if (heartSprites[i].sprite == nullHeart)
            {
                heartSprites[i].sprite = emptyHeart;
                return;
            }
        }
    }


    private void MinusHeart()
    {
        Debug.Log("HEART EVENT");
        for (int i = heartSprites.Count - 1; i >= 0; i--)
        {
            var h = heartSprites[i];

            if (h.sprite == fullHeart)
            {
                h.sprite = emptyHeart;
                return;
            }
        }
    }

    private void OnDestroy()
    {
        if (Player.Instance == null) return;
        Player.Instance.OnApplyDamage -= MinusHeart;
        Player.Instance.OnApplyHeal -= PlusHeart;
    }
}