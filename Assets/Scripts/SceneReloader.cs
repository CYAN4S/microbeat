using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    public void ReloadScene(int code)
    {
        SceneManager.LoadScene(code);
    }
}