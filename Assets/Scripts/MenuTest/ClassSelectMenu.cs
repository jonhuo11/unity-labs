using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ClassSelectMenu : MonoBehaviour
{
    /*
    private static ClassSelectMenu _instance;
    public static ClassSelectMenu Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("no instance of ClassSelectMenu exists");
                return null;
            }
            else
            {
                return _instance;
            }
        }
    }*/

    [SerializeField] RectTransform selectPanelsParent;
    [SerializeField] ClassSelectPanel selectPanelPrefab;
    public Class[] classes = new Class[3];

    private List<ClassSelectPanel> subscribedPanels = new List<ClassSelectPanel>();

    // fill the panel with classes
    void DisplayClasses()
    {
        foreach (Class c in classes)
        {
            if (c == null)
            {
                Debug.LogError("missing class to display, showing less classes than max instead");
            }
            else
            {
                ClassSelectPanel panel = Instantiate(selectPanelPrefab, selectPanelsParent);
                panel.SetDisplayClass(c);

                // subscribe to OnClickEvent
                panel.OnClickEvent += OnPanelClicked;
                subscribedPanels.Add(panel);

                Debug.Log("added panel for class " + c.className);
            }
        }
    }

    // simple callback when a class select panel is clicked
    void OnPanelClicked(Class c)
    {
        Debug.Log(c);
    }

    /*
    void Awake()
    {
        _instance = this;
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        DisplayClasses();
    }

    void OnDisable()
    {
        foreach (ClassSelectPanel p in subscribedPanels)
        {
            p.OnClickEvent -= OnPanelClicked;
        }
        subscribedPanels.Clear();
        Debug.Log("cleared all subscriptions to panel events");
    }
}
