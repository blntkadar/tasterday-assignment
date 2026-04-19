namespace RatioTestApp;

public class SimulatedMeasurementDevice : IMeasurementDevice
{
    public Task<(double, double, double)> MeasureAsync(double testVoltage)
    {
        var rng = new Random();
        double primaryVoltage   = testVoltage   * (1 + 0.005 * (rng.NextDouble() - 0.5));
        double primaryCurrent   = primaryVoltage / 6.0 * (1 + 0.001 * (rng.NextDouble() - 0.5));
        double secondaryVoltage = primaryVoltage / 5   * (1 + 0.001 * (rng.NextDouble() - 0.5));
        return Task.FromResult((primaryVoltage, primaryCurrent, secondaryVoltage));
    }
}