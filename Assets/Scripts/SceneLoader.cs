using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(int code)
    {
        SceneManager.LoadScene(code);
    }
}