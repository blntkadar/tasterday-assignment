namespace RatioTestApp;

public class RatioTest
{
    private readonly IMeasurementDevice _device;

    public RatioTest() : this(new SimulatedMeasurementDevice()) { }
    public RatioTest(IMeasurementDevice device)
    {
        _device = device;
    }

    public double TestVoltage          { get; set; }
    public double RatioVoltagePrimary  { get; set; }
    public double RatioVoltageSecondary { get; set; }

    public async Task<RatioTestResult> RunMeasurement()
    {
        var (primaryVoltage, primaryCurrent, secondaryVoltage) =
            await _device.MeasureAsync(TestVoltage);

        return new RatioTestResult
        {
            ExpectedRatio  = RatioVoltagePrimary / RatioVoltageSecondary,
            PrimaryVoltage   = primaryVoltage,
            PrimaryCurrent   = primaryCurrent,
            SecondaryVoltage = secondaryVoltage
        };
    }
}
