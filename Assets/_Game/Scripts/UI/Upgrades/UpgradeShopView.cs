using System.Collections.Generic;
using UnityEngine;

public class UpgradeShopView : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private UpgradeShopPresenter presenter;
    [SerializeField] private RectTransform itemsRoot;
    [SerializeField] private UpgradeShopRowView rowPrefab;

    [Header("Manual Layout")]
    [SerializeField] private float rowHeight = 80f;
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float paddingLeft = 20f;
    [SerializeField] private float paddingRight = 20f;
    [SerializeField] private float paddingTop = 20f;

    private readonly List<UpgradeShopRowView> _rows = new();

    private void OnEnable()
    {
        if (presenter == null) return;
        presenter.Changed += OnChanged;
        presenter.Refresh();
    }

    private void OnDisable()
    {
        if (presenter == null) return;
        presenter.Changed -= OnChanged;
    }

    private void OnChanged(IReadOnlyList<UpgradeShopItemData> items)
    {
        EnsureRows(items.Count);

        for (int i = 0; i < _rows.Count; i++)
        {
            if (i < items.Count)
            {
                var row = _rows[i];
                row.gameObject.SetActive(true);
                row.Bind(items[i], presenter.Buy);

                LayoutRow(row.GetComponent<RectTransform>(), i);
            }
            else
            {
                _rows[i].gameObject.SetActive(false);
            }
        }
    }

    private void EnsureRows(int count)
    {
        while (_rows.Count < count)
        {
            var row = Instantiate(rowPrefab, itemsRoot);
            _rows.Add(row);
        }
    }

    private void LayoutRow(RectTransform row, int index)
    {
        // anchors: stretch X, top
        row.anchorMin = new Vector2(0f, 1f);
        row.anchorMax = new Vector2(1f, 1f);
        row.pivot = new Vector2(0.5f, 1f);

        float y = paddingTop + index * (rowHeight + spacing);

        row.offsetMin = new Vector2(paddingLeft, -y - rowHeight);
        row.offsetMax = new Vector2(-paddingRight, -y);
    }
}
