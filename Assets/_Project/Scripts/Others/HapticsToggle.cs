using Racer.SaveManager;
using Racer.Utilities;

public class HapticsToggle : ToggleProvider
{
    private void Awake()
    {
        InitToggle();
    }

    protected override void InitToggle()
    {
        ToggleIndex = SaveManager.GetInt(saveString);

        SyncToggle();
    }

    public override void Toggle()
    {
        base.Toggle();

        SaveManager.SaveInt(saveString, ToggleIndex);

        SyncToggle();
    }

    protected override void SyncToggle()
    {
        base.SyncToggle();

        switch (toggleState)
        {
            // default:
            case ToggleState.On:
                Haptics.Mute(false);
                break;
            case ToggleState.Off:
                Haptics.Mute(true);
                break;
        }
    }
}
