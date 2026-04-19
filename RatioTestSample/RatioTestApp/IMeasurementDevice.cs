namespace RatioTestApp;

public interface IMeasurementDevice
{
    Task<(double primaryVoltage, double primaryCurrent, double secondaryVoltage)>
        MeasureAsync(double testVoltage);
}