using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/File IO Event Channel")]
public class FileIOChannelSO : ScriptableObject
{
    public event UnityAction<ChartPath> ChartPathLoadedEvent;

    public void OnChartPathLoaded(ChartPath chartPath)
    {
        ChartPathLoadedEvent?.Invoke(chartPath);
    }
}