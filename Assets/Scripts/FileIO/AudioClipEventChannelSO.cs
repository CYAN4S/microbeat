using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Audio Clip Event Channel")]
public class AudioClipEventChannelSO : ScriptableObject
{
    public event UnityAction<AudioClip> onEventRaised;

    public void RaiseEvent(AudioClip audioClip)
    {
        onEventRaised?.Invoke(audioClip);
    }
}