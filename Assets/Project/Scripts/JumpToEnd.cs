using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class JumpToEnd : MonoBehaviour
{
    public PlayableDirector director;
    public ChangeMainScreen ChangeScreen;

    void Start()
    {
        if (director != null)
            director.stopped += OnTimelineStopped;
    }

    void Update()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            if (director != null)
            {
                director.time = director.duration;
                director.Evaluate();
                ChangeScreen.enabled = true;
            }
        }
    }

    private void OnTimelineStopped(PlayableDirector obj)
    {
        ChangeScreen.enabled = true;
    }
}
