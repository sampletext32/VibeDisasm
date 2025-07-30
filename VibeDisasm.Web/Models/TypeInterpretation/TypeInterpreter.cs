using VibeDisasm.Web.Overlay;

namespace VibeDisasm.Web.Models.TypeInterpretation;

public static class TypeInterpreter
{
    public static TTarget Interpret2<TTarget>(OverlayedType source)
        where TTarget : InterpretValue2
    {
        var interpreter = source.UnderlyingType.Interpreters.FirstOrDefault(x => x.CanInterpretTo<TTarget>(source));

        if (interpreter is null)
        {
            throw new InvalidOperationException(
                $"No interpreter from {source.UnderlyingType.GetType().Name} to {typeof(TTarget).Name} found."
            );
        }

        var interpretValue2 = interpreter.Run(source) as TTarget;

        if (interpretValue2 is null)
        {
            throw new InvalidOperationException(
                $"Unexpected type returned from interpreter {interpretValue2?.GetType().Name}"
            );
        }

        return interpretValue2;
    }

    public static InterpretValue2 InterpretDefault2(OverlayedType source)
    {
        var interpreter = source.UnderlyingType.InterpreterOverride ??
                          source.UnderlyingType.Interpreters.FirstOrDefault()
                          ?? source.UnderlyingType.DefaultInterpreter;

        var interpretValue2 = interpreter.Run(source);

        return interpretValue2;
    }
}
