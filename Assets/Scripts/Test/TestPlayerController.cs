using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct TestDataClass
{
    readonly int hello;
    readonly string epic;
    readonly int id;

    public TestDataClass (int id)
    {
        this.id = id;
        hello = 5;
        epic = "its true";
    }

    public override string ToString()
    {
        return "id:" + id;
    }
}

public class TestPlayerController : NetworkBehaviour
{
    Light lgt;

    [SyncVar(hook = nameof(OnColorChange))]
    Color color;

    readonly SyncList<TestDataClass> testList = new SyncList<TestDataClass>();

    void OnTestListUpdated(SyncList<TestDataClass>.Operation op, int index, TestDataClass oldItem, TestDataClass newItem)
    {
        string output = "";
        foreach (TestDataClass i in testList)
        {
            output += i + " ";
        }
        Debug.Log(output);
    }

    void OnColorChange(Color oldColor, Color newColor)
    {
        if (lgt != null)
        {
            lgt.color = newColor;
        }
    }

    [Command]
    void CmdChangeColor()
    {
        color = Random.ColorHSV();
        TargetReplyChangeColor();
        Debug.Log("this message is displayed on the server side only");
    }
    [TargetRpc]
    void TargetReplyChangeColor()
    {
        Debug.Log("this message is displayed on the client side only");
    }

    [Command]
    void CmdAddToTestList()
    {
        testList.Add(new TestDataClass(Mathf.RoundToInt(Random.Range(0, 999))));
    }

    void Update()
    {
        if (!isLocalPlayer) { return; }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            CmdChangeColor();
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            CmdAddToTestList();
        }
    }

    void Start()
    {
        lgt = GameObject.Find("light").GetComponent<Light>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        testList.Callback += OnTestListUpdated; // adds callback only to client side, server does not care
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        testList.Callback -= OnTestListUpdated;
    }
}
