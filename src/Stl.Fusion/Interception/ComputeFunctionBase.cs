namespace Stl.Fusion.Interception;

public interface IComputeFunction
{
    ComputeMethodDef MethodDef { get; }
    ComputedOptions ComputedOptions { get; }
}

public abstract class ComputeFunctionBase<T> : FunctionBase<T>, IComputeFunction
{
    public ComputeMethodDef MethodDef { get; }
    public ComputedOptions ComputedOptions { get; }

    protected ComputeFunctionBase(ComputeMethodDef methodDef, IServiceProvider services)
        : base(services)
    {
        MethodDef = methodDef;
        ComputedOptions = methodDef.ComputedOptions;
    }

    public override string ToString()
        => MethodDef.FullName;

    // Protected methods

    protected static void SetReturnValue(ComputeMethodInput input, Result<T> output)
    {
        if (input.MethodDef.ReturnsValueTask)
            input.Invocation.ReturnValue =
                // ReSharper disable once HeapView.BoxingAllocation
                output.IsValue(out var v)
                    ? ValueTaskExt.FromResult(v)
                    : ValueTaskExt.FromException<T>(output.Error!);
        else
            input.Invocation.ReturnValue =
                output.IsValue(out var v)
                    ? Task.FromResult(v)
                    : Task.FromException<T>(output.Error!);
    }
}
