using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputReader", menuName = "Blades of the Fallen/Input Reader")]
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
