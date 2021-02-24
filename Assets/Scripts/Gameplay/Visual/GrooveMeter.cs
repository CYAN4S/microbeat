using SO;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Visual
{
    public class GrooveMeter : MonoBehaviour
    {
        [SerializeField] private PlayerSO player;
        [SerializeField] private Image image;

        private void Update()
        {
            image.fillAmount = player.GrooveMeter;
        }
    }
}