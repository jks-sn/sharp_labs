// Utilities/MathUtils.cs

using Fractions;

namespace Hackathon.Utilities;

public static class MathUtils
{
    public static double ComputeHarmonicMean(IEnumerable<int> values)
    {
        if (values == null || !values.Any())
            throw new ArgumentException("Values collection is empty.", nameof(values));

        if (values.Any(v => v == 0))
            throw new ArgumentException("Values cannot contain zero for harmonic mean calculation.", nameof(values));

        return values.Count() / values.Sum(v => 1.0 / v);
    }
}