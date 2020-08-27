﻿using UnityEngine;

[System.Serializable]
public class TransformAnimationStep : IAnimationStep
{
    [SerializeField] private Transform _transform = default;
    [SerializeField] private Vector3 _targetPosition = default;
    [SerializeField] private float _duration = 1f;   
}