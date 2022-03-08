namespace Message;

public static class InnerOpCodeExtensions
{
    public static int Int(this InnerOpCode self)
    {
        return (int) self;
    }
}

public static class OuterOpCodeExtensions
{
    public static int Int(this OuterOpCode self)
    {
        return (int) self;
    }
}

public class Enum<T> where T : struct, IConvertible
{
    public static int Count
    {
        get
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            return Enum.GetNames(typeof(T)).Length;
        }
    }
}