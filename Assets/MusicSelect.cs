using Events;
using UnityEngine;

public class MusicSelect : MonoBehaviour
{
    [SerializeField] private ChartPathEventChannelSO ChartSelectF;

    private void OnEnable()
    {
        ChartSelectF.onEventRaised += StartGame;
    }

    private void OnDisable()
    {
        ChartSelectF.onEventRaised -= StartGame;
    }

    private void StartGame(ChartPath chartPath)
    {
    }
}