using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LifesManager : MonoBehaviour
{
    public List<GameObject> lifesIcons;
    public GameManagerSO gameManager;

    private void OnEnable()
    {
        gameManager.OnPlayerLivesChanged += UpdateLivesUI;
    }

    private void OnDisable()
    {
        gameManager.OnPlayerLivesChanged -= UpdateLivesUI;
    }
    void Start()
    {
        UpdateLifeIcons();
    }
    private void UpdateLivesUI(int lives)
    {
        switch (lives)
        {
            case 3:
                Debug.Log("Player is at full health");
                break;
            case 2:
                lifesIcons[2].SetActive(false);
                break;
            case 1:
                lifesIcons[0].SetActive(false);
                break;
            case 0:
                lifesIcons[1].SetActive(false);
                break;
        }
    }

    private IEnumerator UpdateLifeIcons()
    {
        //wait 1 second per icon
        for (int i = 0; i < lifesIcons.Count; i++)
        {
            lifesIcons[i].SetActive(i < gameManager.playerLives);
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}
