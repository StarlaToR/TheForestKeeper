using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject selectedButton;
    [SerializeField] private GameObject rocks;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject backButton;

    [SerializeField] private float travelTime = 0.5f;
    private float timer = 0f;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        selectedButton = EventSystem.current.currentSelectedGameObject;
        endPosition = startPosition = selectedButton.transform.position;
        rocks.transform.SetPositionAndRotation(startPosition, Quaternion.identity);
    }
    private void Update()
    {
        if (selectedButton != EventSystem.current.currentSelectedGameObject)
        {
            timer = 0f;
            selectedButton = EventSystem.current.currentSelectedGameObject;
            startPosition = rocks.transform.position;
            endPosition = selectedButton.transform.position;
        }

        rocks.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, endPosition, timer / travelTime), Quaternion.identity);

        if (timer < travelTime)
            timer += Time.deltaTime;
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void CreditScreen()
    {
        rocks.SetActive(false);
        mainPanel.SetActive(false);
        creditsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void Back()
    {
        rocks.SetActive(true);
        mainPanel.SetActive(true);
        creditsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}