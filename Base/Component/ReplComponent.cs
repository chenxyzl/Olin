using Base;
using Microsoft.CodeAnalysis.Scripting;

namespace Base.Component;

public class ReplComponent : IGlobalComponent
{
    public ScriptOptions? ScriptOptions;
    public ScriptState? ScriptState;
}