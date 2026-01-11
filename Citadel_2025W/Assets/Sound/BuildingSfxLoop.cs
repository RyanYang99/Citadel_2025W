using UnityEngine;
using System.Collections;

public class SFXLooper : MonoBehaviour
{
    public AudioSource sfx;

    private Coroutine loopRoutine;

    public void PlayLoop(float duration, float pitch)
    {
        if (loopRoutine != null)
            StopCoroutine(loopRoutine);

        loopRoutine = StartCoroutine(LoopCoroutine(duration, pitch));
    }

    private IEnumerator LoopCoroutine(float duration, float pitch)
    {
        sfx.pitch = pitch;

        float end = Time.time + duration;

        while (Time.time < end)
        {
            sfx.Play();
            yield return new WaitForSeconds(1f / pitch);
        }
    }
}