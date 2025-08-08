using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ChangeMainScreen : MonoBehaviour
{
    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                ChangeScreen();
            }
        }
    }

    void ChangeScreen()
    {
        SceneManager.LoadScene("Mechanics", LoadSceneMode.Single);
    }
}
