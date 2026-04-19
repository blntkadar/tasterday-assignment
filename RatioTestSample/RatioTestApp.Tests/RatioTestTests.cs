using Xunit;

namespace RatioTestApp.Tests;

public class StubMeasurementDevice : IMeasurementDevice
{
    private readonly double _primaryVoltage;
    private readonly double _primaryCurrent;
    private readonly double _secondaryVoltage;

    public StubMeasurementDevice(double primaryVoltage, double primaryCurrent, double secondaryVoltage)
    {
        _primaryVoltage   = primaryVoltage;
        _primaryCurrent   = primaryCurrent;
        _secondaryVoltage = secondaryVoltage;
    }

    public Task<(double, double, double)> MeasureAsync(double testVoltage)
        => Task.FromResult((_primaryVoltage, _primaryCurrent, _secondaryVoltage));
}

public class RatioTestResultTests
{
    [Fact]
    public void VoltageRatio_ShouldBe_PrimaryDividedBySecondary()
    {
        // TC-20: ratio = Primary / Secondary
        var result = new RatioTestResult
        {
            PrimaryVoltage   = 10.0,
            SecondaryVoltage = 2.0,
            ExpectedRatio    = 5.0
        };

        Assert.Equal(5.0, result.VoltageRatio, precision: 5);
    }

    [Fact]
    public void RatioDeviation_ShouldBeZero_WhenMeasuredMatchesExpected()
    {
        // TC-22: deviation = 0 when measured ratio equals expected ratio exactly
        var result = new RatioTestResult
        {
            PrimaryVoltage   = 10.0,
            SecondaryVoltage = 2.0,
            ExpectedRatio    = 5.0
        };

        Assert.Equal(0.0, result.RatioDeviation, precision: 5);
    }

    [Fact]
    public void RatioDeviation_ShouldBeCorrectPercentage_WhenMeasuredRatioIsHigher()
    {
        // TC-21: |5.5 - 5.0| / 5.0 * 100 = 10%
        var result = new RatioTestResult
        {
            PrimaryVoltage   = 11.0,  // VoltageRatio = 11/2 = 5.5
            SecondaryVoltage = 2.0,
            ExpectedRatio    = 5.0
        };

        Assert.Equal(10.0, result.RatioDeviation, precision: 3);
    }

    [Fact]
    public void RatioDeviation_ShouldBePositive_WhenMeasuredRatioIsLower()
    {
        // TC-21: |4.5 - 5.0| / 5.0 * 100 = 10% — Math.Abs ensures always positive
        var result = new RatioTestResult
        {
            PrimaryVoltage   = 9.0,   // VoltageRatio = 9/2 = 4.5
            SecondaryVoltage = 2.0,
            ExpectedRatio    = 5.0
        };

        Assert.Equal(10.0, result.RatioDeviation, precision: 3);
    }
}


public class RatioTestMeasurementTests
{
    [Fact]
    public async Task RunMeasurement_ReturnsCorrectExpectedRatio()
    {
        // TC-20: ExpectedRatio = RatioVoltagePrimary / RatioVoltageSecondary
        var stub = new StubMeasurementDevice(10.0, 1.67, 2.0);
        var ratioTest = new RatioTest(stub)
        {
            TestVoltage           = 10,
            RatioVoltagePrimary   = 400,
            RatioVoltageSecondary = 80
        };

        var result = await ratioTest.RunMeasurement();

        Assert.Equal(5.0, result.ExpectedRatio, precision: 5);
    }

    [Fact]
    public async Task RunMeasurement_ReturnsMeasuredValuesFromDevice()
    {
        // TC-23: All measured fields are passed through from the device correctly
        var stub = new StubMeasurementDevice(
            primaryVoltage:   10.05,
            primaryCurrent:   1.675,
            secondaryVoltage: 2.01);

        var ratioTest = new RatioTest(stub)
        {
            TestVoltage           = 10,
            RatioVoltagePrimary   = 400,
            RatioVoltageSecondary = 80
        };

        var result = await ratioTest.RunMeasurement();

        Assert.Equal(10.05, result.PrimaryVoltage,   precision: 5);
        Assert.Equal(1.675, result.PrimaryCurrent,   precision: 5);
        Assert.Equal(2.01,  result.SecondaryVoltage, precision: 5);
    }

    [Fact]
    public async Task RunMeasurement_ProducesNonZeroDeviation_WhenMeasuredDiffersFromExpected()
    {
        // TC-21: stub returns primaryVoltage=10.5, secondary=2.0 -> ratio=5.25
        var stub = new StubMeasurementDevice(
            primaryVoltage:   10.5,
            primaryCurrent:   1.75,
            secondaryVoltage: 2.0);

        var ratioTest = new RatioTest(stub)
        {
            TestVoltage           = 10,
            RatioVoltagePrimary   = 400,
            RatioVoltageSecondary = 80
        };

        var result = await ratioTest.RunMeasurement();

        Assert.Equal(5.0,  result.ExpectedRatio,   precision: 5);
        Assert.Equal(5.25, result.VoltageRatio,    precision: 5);
        Assert.Equal(5.0,  result.RatioDeviation,  precision: 3);
    }

    [Fact]
    public async Task RunMeasurement_ReturnsResult_ForMinimumValidTestVoltage()
    {
        // TC-05: Minimum valid test voltage (0.01V) — result should not be null
        var stub = new StubMeasurementDevice(0.01, 0.001, 0.002);
        var ratioTest = new RatioTest(stub)
        {
            TestVoltage           = 0.01,
            RatioVoltagePrimary   = 100,
            RatioVoltageSecondary = 20
        };

        var result = await ratioTest.RunMeasurement();

        Assert.NotNull(result);
        Assert.Equal(5.0, result.ExpectedRatio, precision: 5);
    }

    [Fact]
    public async Task RunMeasurement_ReturnsResult_ForMaximumValidTestVoltage()
    {
        // TC-04: Maximum valid test voltage (42V) — result should not be null
        var stub = new StubMeasurementDevice(42.0, 7.0, 8.4);
        var ratioTest = new RatioTest(stub)
        {
            TestVoltage           = 42,
            RatioVoltagePrimary   = 400,
            RatioVoltageSecondary = 80
        };

        var result = await ratioTest.RunMeasurement();

        Assert.NotNull(result);
    }
}

public class InputValidationTests
{
    // NOTE: See InputValidator.cs in the main project.

    [Theory]
    [InlineData(0.01)]  // TC-05: minimum valid
    [InlineData(1.0)]   // TC-01: typical
    [InlineData(42.0)]  // TC-04: maximum valid (inclusive boundary)
    public void TestVoltage_ValidValues_ShouldReturnTrue(double voltage)
    {
        var result = InputValidator.ValidateTestVoltage(voltage, out _);
        Assert.True(result);
    }

    [Theory]
    [InlineData(0.0)]   // TC-08: zero
    [InlineData(-1.0)]  // TC-09: negative
    [InlineData(42.01)] // TC-06: just above max (boundary)
    [InlineData(100.0)] // TC-07: clearly above max
    public void TestVoltage_InvalidValues_ShouldReturnFalse(double voltage)
    {
        var result = InputValidator.ValidateTestVoltage(voltage, out _);
        Assert.False(result);
    }

    [Fact]
    public void TestVoltage_InvalidValue_ShouldProvideErrorMessage()
    {
        InputValidator.ValidateTestVoltage(-5.0, out string errorMessage);
        Assert.False(string.IsNullOrEmpty(errorMessage));
    }

    [Theory]
    [InlineData(0.01, 0.01)]
    [InlineData(400,  80)]
    [InlineData(11000, 400)]
    public void NameplateVoltages_BothPositive_ShouldReturnTrue(double primary, double secondary)
    {
        var result = InputValidator.ValidateNameplateVoltages(primary, secondary, out _);
        Assert.True(result);
    }

    [Theory]
    [InlineData(0,    80)]   // TC-12: primary zero
    [InlineData(400,   0)]   // TC-13: secondary zero — division by zero risk
    [InlineData(-400, 80)]   // TC-14: primary negative
    [InlineData(400, -80)]   // secondary negative
    public void NameplateVoltages_InvalidValues_ShouldReturnFalse(double primary, double secondary)
    {
        var result = InputValidator.ValidateNameplateVoltages(primary, secondary, out _);
        Assert.False(result);
    }

    [Fact]
    public void NameplateVoltages_InvalidValue_ShouldProvideErrorMessage()
    {
        InputValidator.ValidateNameplateVoltages(0, 80, out string errorMessage);
        Assert.False(string.IsNullOrEmpty(errorMessage));
    }
}
