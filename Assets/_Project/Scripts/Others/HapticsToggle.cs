using Racer.Utilities;

public class HapticsToggle : SoundToggle
{
    protected override void ApplyToggle()
    {
        switch (toggleState)
        {
            case ToggleState.On:
                Haptics.Mute(false);
                break;
            case ToggleState.Off:
                Haptics.Mute(true);
                break;
        }
    }
}
