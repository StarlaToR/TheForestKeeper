using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public enum State
    {
        PLAY,
        PAUSE,
    }

    private static State state = State.PLAY;

    public static void ChangeState(State newState)
    {
        state = newState;
    }

    public static void ChangeScene(int index)
    {
        TimeManager.ChangeGameSpeed(1);
        
        SceneManager.LoadScene(index);
    }
    public static State GetState() { return state; }
}
