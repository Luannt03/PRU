using System;
using UnityEngine;
using ChemLab9.Core;
namespace ChemLab9.Interaction
{
    public class Interactable : MonoBehaviour
    {
        public StationController Station;
        public string Label;
        public Action Click;
        public virtual void Activate()
        {
            if (Station == null || !Station.Active || !Station.Lesson.Started) return;
            Click?.Invoke();
        }
    }
}
