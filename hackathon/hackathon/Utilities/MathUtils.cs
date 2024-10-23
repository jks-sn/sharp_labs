namespace Hackathon.Utilities;

public static class MathUtils
{
    public static decimal ComputeHarmonicMean(IEnumerable<int> values)
    {
        int n = values.Count(); 
        decimal denominator = values.Sum(v => 1m / v);
        return n / denominator;
    }
}