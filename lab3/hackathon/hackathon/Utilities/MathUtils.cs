// File: Utilities/MathUtils.cs

using Fractions;

namespace Hackathon.Utilities;

public static class MathUtils
{
    public static double ComputeHarmonicMean(IEnumerable<int> values)
    {
        if (values == null || !values.Any())
            throw new ArgumentException("Коллекция значений не может быть пустой или null.", nameof(values));

        int n = values.Count();
        Fraction sum = Fraction.Zero;

        foreach (var v in values)
        {
            if (v == 0)
            {
                throw new ArgumentException("Значения не могут быть равны нулю для вычисления гармонического среднего.",
                    nameof(values));
            }

            sum += Fraction.One / v;
        }

        Fraction harmonicMean = n / sum;
        return (double)harmonicMean;
    }
}