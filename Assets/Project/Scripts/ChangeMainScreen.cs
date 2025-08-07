using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeMainScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if new input sistem touch screen is pressed change the main screen
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ChangeScreen();
        }
    }

    void ChangeScreen()
    {
        SceneManager.LoadScene("Mechanics", LoadSceneMode.Single);
    }
}
