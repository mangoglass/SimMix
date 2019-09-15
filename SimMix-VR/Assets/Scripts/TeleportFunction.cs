
public class TeleportFunction : IFunction 
{
    public bool Call(IInputParser input) 
    {
        return input.TeleportBool();
    }
}
