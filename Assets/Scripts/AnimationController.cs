using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeReference] private List<IAnimationStep> _animationSteps = new List<IAnimationStep>();

    private void Start()
    {
        CompileSequence();
    }

    public Sequence CompileSequence()
    {
        var seq = DOTween.Sequence();
        _animationSteps.ForEach(step => seq.Append(step.Tween));
        return seq;
    }
}