using UnityEngine;
using UnityEngine.Events;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
    public class InputReader : ScriptableObject
    {
        // Gameplay
        public event UnityAction<int> SpeedEvent;
        public event UnityAction<int> PlayKeyDownEvent;
        public event UnityAction<int> PlayKeyEvent;
        public event UnityAction<int> PlayKeyUpEvent;
        public event UnityAction<int> PlayKeyInterruptEvent;
        public event UnityAction PauseKeyEvent;

        public void OnSpeed(int key)
        {
            SpeedEvent?.Invoke(key);
        }

        public void OnPlayKeyDown(int key)
        {
            PlayKeyDownEvent?.Invoke(key);
        }

        public void OnPlayKey(int key)
        {
            PlayKeyEvent?.Invoke(key);
        }

        public void OnPlayKeyUp(int key)
        {
            PlayKeyUpEvent?.Invoke(key);
        }

        public void OnPlayKeyInterrupt(int key)
        {
            PlayKeyInterruptEvent?.Invoke(key);
        }

        public void OnPause()
        {
            PauseKeyEvent?.Invoke();
        }
    }
}