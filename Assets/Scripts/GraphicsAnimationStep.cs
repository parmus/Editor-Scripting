using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GraphicAnimationStep : IAnimationStep
{
    [SerializeField] private Graphic _graphic = default;
    [SerializeField] private Color _targetColor;
    [SerializeField] private float _duration = 1f;   
}