using System;
using Gameplay.Visual;
using SO.DataContainer;
using UnityEngine;

namespace SO.ParticularChannel
{
    [CreateAssetMenu(menuName = "Skin Selection")]
    public class SkinSelectionSO : ScriptableObject
    {
        public GearSet gearSet;
    }
}