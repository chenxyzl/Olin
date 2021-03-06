using Message;

namespace Base;

public static class A
{
    //可预料的错误 会把错误码返回客户端
    public static void Ensure(bool a, string? des = null, Code code = Code.Error, bool serious = false)
    {
        if (a != true) throw new CodeException(code, des ?? code.ToString(), serious);
    }

    //可预料的错误 会把错误码返回客户端
    public static void Abort(string? des = null, Code code = Code.Error, bool serious = false)
    {
        throw new CodeException(code, des ?? code.ToString(), serious);
    }

    //可预料的错误 会把错误码返回客户端
    public static T NotNull<T>(T? t, string? des = null, Code code = Code.Error, bool serious = false)
    {
        return t ?? throw new CodeException(code, des ?? $"{nameof(t)}: must not null", serious);
    }
}