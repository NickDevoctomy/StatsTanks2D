using System;
using UnityEngine;

[Serializable]
public struct MultiSampleInfo
{
    public string Key;
    public AudioClip AudioClip;
    public bool Loop;
    public float AttackTime;
    public float ReleaseTime;
}
