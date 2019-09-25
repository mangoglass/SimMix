using UnityEngine;

public interface IInputParser
{
    bool ToolBool();
    bool ToolBoolDown();
    bool ToolBoolUp();
    bool ToolTriggerValueChanged();

    bool TeleportBool();
    bool TeleportBoolDown();
    bool TeleportBoolUp();

    bool SwapBool();

    bool MenuDisplayBool();
    bool MenuDisplayBoolDown();
    bool MenuDisplayBoolUp();
    bool MenuClickBool();

    bool isLeftController();
    bool isRightController();

    float ToolTriggerValue();
    Vector2 MenuPointerLocation();
    float ToolLastTriggerValue();

    Transform GetTransform();
}
