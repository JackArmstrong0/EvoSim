using UnityEngine;

public class UIManager : MonoBehaviour
{
    public RectTransform panel;
    public float slideSpeed = 50f;
    private bool isVisible = false;
    private Vector2 hiddenPosition = new Vector2(763f, -800f);
    private Vector2 visiblePosition = new Vector2(763f, -183f);
    Vector2 targetPosition;

    void Start()
    {
        panel.anchoredPosition = hiddenPosition;
    }

    void Update()
    {
        if (isVisible)
        {
            targetPosition = visiblePosition;
        }
        else
        {
            targetPosition = hiddenPosition;
        }
        // Lerp causes easing, so it will slow down as it approaches the target position
        panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, targetPosition, slideSpeed * Time.unscaledDeltaTime);
    }
    public void Show()
    {
        isVisible = true;
    }

    public void Hide()
    {
        isVisible = false;
    }
}
