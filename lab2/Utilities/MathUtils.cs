namespace Hackathon.Utilities;

public static class MathUtils
{
    public static double ComputeHarmonicMean(IEnumerable<int> values)
    {
        if (values == null || !values.Any())
            throw new ArgumentException("Коллекция значений не может быть пустой или null.", nameof(values));

        int n = values.Count(); 
        double denominator = values.Sum(v =>
        {
            if (v == 0)
            {
                throw new ArgumentException("Значения не могут быть равны нулю для вычисления гармонического среднего.", nameof(values));
            }
            return 1.0 / v;
        });
        
        return n / denominator;
    }
}