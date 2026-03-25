using UnityEngine;

public class UIManager : MonoBehaviour
{
    public RectTransform panel;
    public float slideSpeed = 50f;
    public bool isVisible = false;
    public Vector2 hiddenPosition;
    public Vector2 visiblePosition;
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
