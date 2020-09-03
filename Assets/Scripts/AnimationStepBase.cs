using DG.Tweening;
using UnityEngine;

public abstract class AnimationStepBase : IAnimationStep
{
    public abstract Tween Tween { get; }
}