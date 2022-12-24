using Racer.SaveSystem;
using Racer.SoundManager;
using Racer.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : ToggleProvider
{
    [Space(5), Header("TARGET GRAPHICS")]
    public Image parentIcon;
    public Sprite[] onOffIcons;


    private void Awake()
    {
        InitToggle();
    }

    protected override void InitToggle()
    {
        ToggleIndex = SaveSystem.GetData<int>(saveString);

        SyncToggle();
    }

    public override void Toggle()
    {
        base.Toggle();

        SaveSystem.SaveData(saveString, ToggleIndex);

        SyncToggle();
    }

    protected override void SyncToggle()
    {
        base.SyncToggle();

        parentIcon.sprite = onOffIcons[ToggleIndex];
    }

    protected override void ApplyToggle()
    {
        switch (toggleState)
        {
            case ToggleState.On:
                SoundManager.Instance.EnableMusic(true);
                SoundManager.Instance.EnableSfx(true);
                break;
            case ToggleState.Off:
                SoundManager.Instance.EnableMusic(false);
                SoundManager.Instance.EnableSfx(false);
                break;
        }
    }
}