using UnityEngine;

public class Utility
{
    public static Outline CreateOutline(GameObject go, Color color, bool enabled) {
        Outline outline = go.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.enabled = enabled;
        outline.OutlineColor = color;
        outline.OutlineWidth = 6f;
        return outline;
    }
}
