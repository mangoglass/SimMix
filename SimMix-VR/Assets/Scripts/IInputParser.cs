using UnityEngine;

public interface IInputParser
{
    bool ToolBool();
    bool ToolBoolDown();
    bool ToolBoolUp();
    bool ToolTriggerValueChanged();

    bool TeleportBool();

    bool HeadsetBool();
    bool HeadsetBoolUp();

    bool SwapBool();

    bool MenuDisplayBool();
    bool MenuDisplayBoolDown();
    bool MenuDisplayBoolUp();
    bool MenuClickBool();

    bool isLeftController();
    bool isRightController();

    float ToolTriggerValue();
    float ToolLastTriggerValue();
    Vector2 MenuTrackLocation();
    Transform GetTransform();
}
