using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class JumpToEnd : MonoBehaviour
{
    public PlayableDirector director;

    void Update()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            if (director != null)
            {
                director.time = director.duration;
                director.Evaluate();
            }
        }
    }
}
