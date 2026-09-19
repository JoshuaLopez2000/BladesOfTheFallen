using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class JumpToEnd : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private void Update()
    {
        if (director == null || Touchscreen.current == null)
        {
            return;
        }

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            director.time = director.duration;
            director.Evaluate();
        }
    }
}
