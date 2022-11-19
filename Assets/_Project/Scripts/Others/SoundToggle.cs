using Racer.SoundManager;
using Racer.Utilities;

public class SoundToggle : ToggleProvider
{
    private void Awake()
    {
        InitToggle();
    }

    protected override void InitToggle()
    {
        // ToggleIndex = SaveManager.GetInt(saveString);

        SyncToggle();
    }

    public override void Toggle()
    {
        base.Toggle();

        // SaveManager.SaveInt(saveString, ToggleIndex);

        SyncToggle();
    }


    protected override void SyncToggle()
    {
        base.SyncToggle();

        switch (toggleState)
        {
            // default:
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