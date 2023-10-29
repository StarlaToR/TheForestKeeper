using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject panel;
    public GameObject firstButton;
    public bool opened = false;

    private GameObject selectedButton;
    [SerializeField] private GameObject rocks;
    public Volume m_postProcess;

    private float travelTime = 0.5f;
    private float timer = 0f;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        selectedButton = EventSystem.current.currentSelectedGameObject;
        endPosition = startPosition = selectedButton.transform.position;
        rocks.transform.SetPositionAndRotation(startPosition, Quaternion.identity);

        panel.SetActive(false);
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

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

    void OpenPauseMenu()
    {
        panel.SetActive(true);
        opened = true;
        GameState.ChangeState(GameState.State.PAUSE);
        EventSystem.current.SetSelectedGameObject(firstButton);
        m_postProcess.weight = 1f;
    }

    void ClosePauseMenu()
    {
        panel.SetActive(false);
        opened = false;
        GameState.ChangeState(GameState.State.PLAY);
        m_postProcess.weight = 0f;
    }

    public void SetPauseMenu()
    {
        if (opened)
            ClosePauseMenu();
        else
            OpenPauseMenu();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
