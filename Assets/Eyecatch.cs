using System.Collections;
using System.IO;
using Core;
using Core.SO.NormalChannel;
using FileIO;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Eyecatch : MonoBehaviour
{
    [SerializeField] private ChartEventChannelSO onChartSelect;
    [SerializeField] private GameObject mask;
    [SerializeField] private RawImage image;

    private void OnEnable()
    {
        onChartSelect.OnEventRaised += Transfer;
    }

    private void OnDisable()
    {
        onChartSelect.OnEventRaised -= Transfer;
    }

    private void Transfer(Chart chart)
    {
        var path = chart.desc.imgPath;

        if (path != null)
        {
            path = Path.Combine(chart.directoryPath, path);
            StartCoroutine(Serialize.GetTexture(path, (value) =>
            {
                image.texture = value;
                mask.SetActive(true);
            }));
        }
        else
        {
            mask.SetActive(true);
        }

        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2);
    }
}