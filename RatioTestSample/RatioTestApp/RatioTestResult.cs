namespace RatioTestApp;

public class RatioTestResult
{
    public double ExpectedRatio    { get; set; }
    public double PrimaryVoltage   { get; set; }
    public double PrimaryCurrent   { get; set; }
    public double SecondaryVoltage { get; set; }

    // BUGFIX 1
    public double VoltageRatio => PrimaryVoltage / SecondaryVoltage;

    // BUGFIX 2
    public double RatioDeviation => Math.Abs(VoltageRatio - ExpectedRatio) / ExpectedRatio * 100;
}