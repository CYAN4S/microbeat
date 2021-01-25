using System.Linq;
using SO;
using TMPro;
using UnityEngine;

public class Combo : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Animator[] animators;

    private void OnEnable()
    {
        player.ComboIncreaseEvent += Animate;
    }

    private void OnDisable()
    {
        player.ComboIncreaseEvent -= Animate;
    }

    private void Animate()
    {
        comboText.text = player.Combo.ToString().Aggregate("", (current, c) => current + $"<sprite={c}>");
        foreach (var animator in animators) animator.SetTrigger("Combo");
    }
}