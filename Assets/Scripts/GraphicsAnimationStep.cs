using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GraphicAnimationStep : AnimationStepBase
{
    [SerializeField] private Graphic _graphic = default;
    [SerializeField] private Color _targetColor;
    [SerializeField] private float _duration = 1f;

    public override Tween Tween => _graphic.DOColor(_targetColor, _duration);
}