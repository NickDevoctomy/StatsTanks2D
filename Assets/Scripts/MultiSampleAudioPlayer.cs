using System.Collections.Generic;
using UnityEngine;

public class MultiSampleAudioPlayer : MonoBehaviour
{
    public MultiSampleInfo[] Samples;

    private Dictionary<string, MultiSampleContext> _contexts;

    void Start()
    {
        CreateContexts();
    }

    private void CreateContexts()
    {
        _contexts= new Dictionary<string, MultiSampleContext>();
        foreach(var curSample in Samples)
        {

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = curSample.AudioClip;
            audioSource.loop = curSample.Loop;
            audioSource.outputAudioMixerGroup = curSample.MixerGroup;
            audioSource.minDistance = curSample.MinDistance;
            audioSource.maxDistance = curSample.MaxDistance;
            audioSource.dopplerLevel = 0.25f;
            audioSource.spatialBlend = 1f;
            audioSource.spatialize = true;
            var context = new MultiSampleContext
            {
                Info = curSample,
                AudioSource = audioSource,
            };

            _contexts.Add(curSample.Key, context);
        }
    }

    public void PlayWithAttackAndRelease(
        string key,
        bool active)
    {
        if (_contexts == null)
        {
            return;
        }

        if (!_contexts.ContainsKey(key))
        {
            return;
        }

        var context = _contexts[key];
        if(!context.Info.Enabled)
        {
            return;
        }

        var audioSource = context.AudioSource;

        if (active)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.volume = 0f;
                context.CurrentPlaybackTime = 0f;
                audioSource.Play();
            }
            else
            {
                context.FadeOut = false;
                context.CurrentPlaybackTime += Time.deltaTime;
                var newVolume = context.CurrentPlaybackTime / context.Info.AttackTime;
                audioSource.volume = newVolume;
            }
        }
        else
        {
            if (!context.FadeOut &&
                audioSource.isPlaying)
            {
                context.CurrentPlaybackTime = 0f;
                context.FadeOut = true;
            }

            if (context.FadeOut)
            {
                context.CurrentPlaybackTime += Time.deltaTime;
                var newVolume = context.CurrentPlaybackTime / context.Info.ReleaseTime;
                audioSource.volume = 1.0f - newVolume;

                if (context.CurrentPlaybackTime >= context.Info.ReleaseTime)
                {
                    audioSource.Stop();
                    audioSource.volume = 0f;
                    context.CurrentPlaybackTime = 0f;
                    context.FadeOut = false;
                }
            }
        }
    }

}
