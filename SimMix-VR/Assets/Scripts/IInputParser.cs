using UnityEngine;

public interface IInputParser
{
    bool ToolBoolCheck();
    bool ToolBoolReleased();
    bool ToolTriggerValueChanged();
    bool TeleportBoolCheck();
    bool HeadsetBoolCheck();
    bool HeadsetBoolReleased();
    bool SwapBoolCheck();
    bool MenuDisplayBoolCheck();
    bool MenuClickBoolCheck();
    bool isLeftController();
    bool isRightController();

    float ToolTriggerValue();
    Vector2 MenuTrackLocation();
    Transform GetTransform();
}
