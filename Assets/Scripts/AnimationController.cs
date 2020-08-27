using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeReference] private List<IAnimationStep> _animationSteps = new List<IAnimationStep>();
}