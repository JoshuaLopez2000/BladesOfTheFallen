using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReaderSO", menuName = "Scriptable Objects/InputReaderSO")]
public class InputReaderSO : ScriptableObject
{
    public event Action OnSlashRight;
    public event Action OnSlashLeft;
    public event Action OnParry;

    public void RaiseSlashRight()
    {
        OnSlashRight?.Invoke();
    }

    public void RaiseSlashLeft()
    {
        OnSlashLeft?.Invoke();
    }

    public void RaiseParry()
    {
        OnParry?.Invoke();
    }
}
