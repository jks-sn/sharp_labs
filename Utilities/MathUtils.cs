namespace Hackathon.Utilities;

public static class MathUtils
{
    public static double ComputeHarmonicMean(IEnumerable<int> values)
    {
        int n = values.Count();
        double denominator = values.Sum(v => 1.0 / v);
        return n / denominator;
    }
}