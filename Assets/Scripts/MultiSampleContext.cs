using UnityEngine;

public class MultiSampleContext
{
    public MultiSampleInfo Info { get; set; }
    public AudioSource AudioSource { get; set; }
    public float CurrentPlaybackTime { get; set; } = 0f;
    public bool FadeOut { get; set; }
}
