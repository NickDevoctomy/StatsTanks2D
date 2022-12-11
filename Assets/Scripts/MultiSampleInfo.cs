using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct MultiSampleInfo
{
    public bool Enabled;
    public string Key;
    public AudioClip AudioClip;
    public AudioMixerGroup MixerGroup;
    public bool Loop;
    public float AttackTime;
    public float ReleaseTime;
}
