using UnityEngine;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    public bool askBeforeClose;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onOpen;
    [SerializeField] private UnityEvent onClose;
    
    public void Open()
    {
        gameObject.SetActive(true);
        onOpen?.Invoke();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        onClose?.Invoke();
    }
}
