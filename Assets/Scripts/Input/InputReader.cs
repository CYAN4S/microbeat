using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject
{
    // Gameplay
    public event UnityAction<int> speedEvent;
    public event UnityAction<int> playKeyDownEvent;
    public event UnityAction<int> playKeyEvent;
    public event UnityAction<int> playKeyUpEvent;
    public event UnityAction feverKeyEvent;
    public event UnityAction pauseKeyEvent;

    public void OnSpeed(int key)
    {
        if (speedEvent != null)
            speedEvent.Invoke(key);
        else
            Debug.LogWarning("No events in InputReader.speedEvent");
    }

    public void OnPlayKeyDown(int key)
    {
        if (playKeyDownEvent != null)
            playKeyDownEvent.Invoke(key);
        else
            Debug.LogWarning("No events in InputReader.playKeyDownEvent");
    }

    public void OnPlayKey(int key)
    {
        if (playKeyEvent != null)
            playKeyEvent.Invoke(key);
        else
            Debug.LogWarning("No events in InputReader.playKeyEvent");
    }

    public void OnPlayKeyUp(int key)
    {
        if (playKeyUpEvent != null)
            playKeyUpEvent.Invoke(key);
        else
            Debug.LogWarning("No events in InputReader.playKeyUpEvent");
    }

    public void OnFever()
    {
        if (feverKeyEvent != null)
            feverKeyEvent.Invoke();
        else
            Debug.LogWarning("No events in InputReader.feverKeyEvent");
    }

    public void OnPause()
    {
        if (pauseKeyEvent != null)
            pauseKeyEvent.Invoke();
        else
            Debug.LogWarning("No events in InputReader.pauseKeyEvent");
    }
}