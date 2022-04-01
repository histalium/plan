using OneOf;

namespace Plan.Utilities;

public static class OneOfWithErrorExtensions
{
    public static OneOf<T2, ErrorMessage> Map<T1, T2>(this OneOf<T1, ErrorMessage> input1, Func<T1, T2> transform)
    {
        return input1.Match<OneOf<T2, ErrorMessage>>(
            t1 => transform(t1),
            e => e);
    }

    public static OneOf<T3, ErrorMessage> ValuesOrError<T1, T2, T3>(this OneOf<T1, ErrorMessage> input1, OneOf<T2, ErrorMessage> input2, Func<T1, T2, T3> transform)
    {
        return input2.Match(
            t2 => input1.Match<OneOf<T3, ErrorMessage>>(
                t1 => transform(t1, t2),
                e => e),
            e => e);
    }

    public static OneOf<(T1, T2), ErrorMessage> TupleOrError<T1, T2>(this OneOf<T1, ErrorMessage> input1, OneOf<T2, ErrorMessage> input2)
    {
        return input1.ValuesOrError(input2, (i1, i2) => (i1, i2));
    }

    public static OneOf<(T1, T2, T3), ErrorMessage> TupleOrError<T1, T2, T3>(this OneOf<(T1, T2), ErrorMessage> input1, OneOf<T3, ErrorMessage> input2)
    {
        return input1.ValuesOrError(input2, (t, i2) => (t.Item1, t.Item2, i2));
    }

    public static OneOf<(T1, T2, T3, T4), ErrorMessage> TupleOrError<T1, T2, T3, T4>(this OneOf<(T1, T2, T3), ErrorMessage> input1, OneOf<T4, ErrorMessage> input2)
    {
        return input1.ValuesOrError(input2, (t, i2) => (t.Item1, t.Item2, t.Item3, i2));
    }

    public static OneOf<(T1, T2, T3, T4, T5), ErrorMessage> TupleOrError<T1, T2, T3, T4, T5>(this OneOf<(T1, T2, T3, T4), ErrorMessage> input1, OneOf<T5, ErrorMessage> input2)
    {
        return input1.ValuesOrError(input2, (t, i2) => (t.Item1, t.Item2, t.Item3, t.Item4, i2));
    }
}
