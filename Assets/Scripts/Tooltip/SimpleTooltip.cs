/*
 * attach script to tooltip prefab, can create customizeable tooltips depending on trigger source
 */


using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(VerticalLayoutGroup))]
[RequireComponent(typeof(RectTransform))]
public class SimpleTooltip : MonoBehaviour
{
    public static SimpleTooltip Instance { get; private set; }

    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] TextMeshProUGUI contentText;
    public int characterWrapLimit;
    public float popupAfterSeconds;

    LayoutElement layoutElement;
    RectTransform rectTransform;

    public void UpdateTooltip(SimpleTooltipTrigger t)
    {
        UpdateTooltip(t.content, t.header);
    }

    public void UpdateTooltip(string content, string header = null)
    {

        if (string.IsNullOrEmpty(header))
        {
            headerText.gameObject.SetActive(false);
        }
        else
        {
            headerText.gameObject.SetActive(true);
            headerText.SetText(header);
        }
        contentText.SetText(content);

        ResizeTooltip();
        MoveTooltipToMouse();
    }

    void ResizeTooltip()
    {
        int headerLength = headerText.text.Length;
        int contentLength = contentText.text.Length;

        layoutElement.enabled = headerLength > characterWrapLimit || contentLength > characterWrapLimit;
    }

    void MoveTooltipToMouse()
    {
        Vector2 mousePos = (Vector2)Input.mousePosition;
        // change position around mouse based on where screen edges are
        float x = mousePos.x / Screen.width;
        float y = mousePos.y / Screen.height;
        int x1 = x >= 0.5f ? 1 : 0;
        int y1 = y >= 0.5f ? 1 : 0;
        rectTransform.pivot = new Vector2(x1, y1);

        transform.position = mousePos;
    }

    void Update()
    {
        MoveTooltipToMouse();
    }

    void Start()
    {
        layoutElement = GetComponent<LayoutElement>();
        rectTransform = GetComponent<RectTransform>();
        ResizeTooltip();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("found existing instance of TooltipManager, deleting this instance");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}