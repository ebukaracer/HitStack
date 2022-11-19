using Racer.SoundManager;
using Racer.Utilities;
using UnityEngine;

internal class PlayerProperties : MonoBehaviour
{
    [Header("PARTICLE FX")]
    [SerializeField] private ParticleSystem splashFx;
    [SerializeField] private ParticleSystem winFx;
    [SerializeField] private ParticleSystem powerupFx;

    [Header("ANIMATOR"), Space(5)]
    [SerializeField] private Animator checkpointAnimator;

    [Header("AUDIO CLIPS"), Space(5)]
    [SerializeField] private AudioClip winSfx;
    [SerializeField] private AudioClip powerupSfx;
    [SerializeField] private AudioClip deepBounceSfx;
    [SerializeField] private AudioClip loseSfx;
    [SerializeField] private AudioClip bounceSfx;

    [SerializeField, Space(5)]
    private float splashFxOffset = -0.15f;

    private static readonly int Scale = Animator.StringToHash("Scale");


    public void OnPlayerBounce(bool hasClicked)
    {
        if (hasClicked)
            OnHitStack();
        else
            OnBounce();
    }

    // Deep bounce
    private void OnHitStack()
    {
        splashFx.Clear();

        splashFx.Stop();

        SoundManager.Instance.PlaySfx(deepBounceSfx, .25f);
    }

    // Normal bounce
    private void OnBounce()
    {
        splashFx.transform.position = transform.position + Vector3.up * splashFxOffset;

        splashFx.Play();

        SoundManager.Instance.PlaySfx(bounceSfx, .25f);
    }

    public void OnHitCheckpoint(GameObject go)
    {
        winFx.transform.position = new Vector3
            (transform.position.x, go.transform.position.y + .1f, transform.position.z);

        winFx.Play();

        SoundManager.Instance.PlaySfx(winSfx);

        checkpointAnimator.SetTrigger(Scale);
    }

    public void OnPlayerPowerUp(bool status)
    {
        if (status)
        {
            powerupFx.Play();

            Haptics.Vibrate(250);

            SoundManager.Instance.PlaySfx(powerupSfx, .5f);
        }
        else
            powerupFx.Stop();
    }

    public void OnPlayerLost()
    {
        SoundManager.Instance.PlaySfx(loseSfx);
    }
}

