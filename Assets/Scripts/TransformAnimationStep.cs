using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class TransformAnimationStep : AnimationStepBase
{
    [SerializeField] private Transform _transform = default;
    [SerializeField] private Vector3 _targetPosition = default;
    [SerializeField] private Quaternion _targetOrientarion = default;
    [SerializeField] private Vector3 _targetScale = default;
    [SerializeField] private float _duration = 1f;

    public override Tween Tween => _transform.DOMove(_targetPosition, _duration);
}