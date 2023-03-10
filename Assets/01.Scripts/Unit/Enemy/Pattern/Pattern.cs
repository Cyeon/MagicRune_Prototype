using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Pattern
{
    public float value;
    public PatternSO info;

    public void Start()
    {
        UIManager.Instance.ReloadPattern(info.icon, value > 0 ? value.ToString() : "");
        PatternManager.Instance.funcList.value = value;
        PatternManager.Instance.FuncInovke(info.startFunc);
    }

    public void Turn()
    {
        PatternManager.Instance.FuncInovke(info.turnFunc);
    }

    public void End()
    {
        PatternManager.Instance.FuncInovke(info.endFunc);
    }
}
