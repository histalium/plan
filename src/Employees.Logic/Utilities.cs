namespace Employees.Logic;

internal static class Utilities
{
    internal static OneOf<T2, Error> Map<T1, T2>(this OneOf<T1, Error> input1, Func<T1, T2> transform)
    {
        return input1.Match<OneOf<T2, Error>>(
            t1 => transform(t1),
            e => e);
    }

    internal static OneOf<T3, Error> ValuesOrError<T1, T2, T3>(this OneOf<T1, Error> input1, OneOf<T2, Error> input2, Func<T1, T2, T3> transform)
    {
        return input2.Match(
            t2 => input1.Match<OneOf<T3, Error>>(
                t1 => transform(t1, t2),
                e => e),
            e => e);
    }

    internal static OneOf<(T1, T2), Error> TupleOrError<T1, T2>(this OneOf<T1, Error> input1, OneOf<T2, Error> input2)
    {
        return input1.ValuesOrError(input2, (i1, i2) => (i1, i2));
    }

    internal static OneOf<(T1, T2, T3), Error> TupleOrError<T1, T2, T3>(this OneOf<(T1, T2), Error> input1, OneOf<T3, Error> input2)
    {
        return input1.ValuesOrError(input2, (t, i2) => (t.Item1, t.Item2, i2));
    }

    internal static OneOf<(T1, T2, T3, T4), Error> TupleOrError<T1, T2, T3, T4>(this OneOf<(T1, T2, T3), Error> input1, OneOf<T4, Error> input2)
    {
        return input1.ValuesOrError(input2, (t, i2) => (t.Item1, t.Item2, t.Item3, i2));
    }

    internal static OneOf<(T1, T2, T3, T4, T5), Error> TupleOrError<T1, T2, T3, T4, T5>(this OneOf<(T1, T2, T3, T4), Error> input1, OneOf<T5, Error> input2)
    {
        return input1.ValuesOrError(input2, (t, i2) => (t.Item1, t.Item2, t.Item3, t.Item4, i2));
    }
}
