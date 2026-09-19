using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LivesManager : MonoBehaviour
{
    [FormerlySerializedAs("lifesIcons")]
    [SerializeField] private List<GameObject> lifeIcons = new();
    [SerializeField] private GameManagerSO gameManager;

    private void OnEnable()
    {
        gameManager.OnPlayerLivesChanged += UpdateLivesUI;
    }

    private void OnDisable()
    {
        gameManager.OnPlayerLivesChanged -= UpdateLivesUI;
    }
    private void Start()
    {
        UpdateLivesUI(gameManager.PlayerLives);
    }

    private void UpdateLivesUI(int lives)
    {
        for (int index = 0; index < lifeIcons.Count; index++)
        {
            if (lifeIcons[index] != null)
            {
                lifeIcons[index].SetActive(index < lives);
            }
        }
    }
}
