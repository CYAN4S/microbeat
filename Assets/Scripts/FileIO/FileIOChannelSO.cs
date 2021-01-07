using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/File IO Event Channel")]
public class FileIOChannelSO : ScriptableObject
{
    public event UnityAction<List<Musicpack>> listLoadFinishedEvent;
    public event UnityAction<Musicpack, SerializableSheet> chartLoadedEvent;

    public void OnListLoadFinished(List<Musicpack> list)
    {
        listLoadFinishedEvent?.Invoke(list);
    }

    public void OnChartLoaded(Musicpack music, SerializableSheet sheet)
    {
        chartLoadedEvent?.Invoke(music, sheet);
    }
}
