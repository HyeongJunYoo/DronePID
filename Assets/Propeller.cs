using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
    [field:SerializeField] public float Yaw { get; set; }
    [field:SerializeField] public float Pitch { get; set; }
    [field:SerializeField] public float Roll { get; set; }
    [field:SerializeField] public float Power { get; set; }

    public Animator animator;
    private static readonly int AnimatorPower = Animator.StringToHash("Power");

    public void PowerControl(float value)
    {
        Power = Yaw + Pitch + Roll + value;
    }

    public void Update()
    {
        animator.SetFloat(AnimatorPower, Power / 2.3f);
    }
}
