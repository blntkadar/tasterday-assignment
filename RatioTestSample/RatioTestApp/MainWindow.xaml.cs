using System.Globalization;
using System.Windows;

namespace RatioTestApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void RunMeasurementButton_OnClick(object sender, RoutedEventArgs e)
    {
        ErrorTextBlock.Text                  = string.Empty;
        MeasuredPrimaryVoltageTextBlock.Text  = string.Empty;
        MeasuredPrimaryCurrentTextBlock.Text  = string.Empty;
        MeasuredSecondaryVoltageTextBlock.Text = string.Empty;
        ExpectedVoltageRatioTextBlock.Text    = string.Empty;
        VoltageRatioTextBlock.Text            = string.Empty;
        RatioDeviationTextBlock.Text          = string.Empty;

        // Validate and parse input
        if (!TryParseInput(out double testVoltage,
                           out double ratedPrimaryVoltage,
                           out double ratedSecondaryVoltage,
                           out string errorMessage))
        {
            ErrorTextBlock.Text = errorMessage;
            return;
        }

        // Create and run the ratio test
        var ratioTest = new RatioTest
        {
            TestVoltage             = testVoltage,
            RatioVoltagePrimary     = ratedPrimaryVoltage,
            RatioVoltageSecondary   = ratedSecondaryVoltage
        };

        var result = await ratioTest.RunMeasurement();
        if (result != null)
        {
            MeasuredPrimaryVoltageTextBlock.Text   = "Primary Voltage: "   + result.PrimaryVoltage.ToString("F2")   + " V";
            MeasuredPrimaryCurrentTextBlock.Text   = "Primary Current: "   + result.PrimaryCurrent.ToString("F2")   + " A";
            MeasuredSecondaryVoltageTextBlock.Text = "Secondary Voltage: " + result.SecondaryVoltage.ToString("F2") + " V";
            ExpectedVoltageRatioTextBlock.Text     = "Expected Voltage Ratio: " + result.ExpectedRatio.ToString("F2");
            VoltageRatioTextBlock.Text             = "Voltage Ratio: "     + result.VoltageRatio.ToString("F2");
            RatioDeviationTextBlock.Text           = "Ratio Deviation: "   + result.RatioDeviation.ToString("F2")   + " %";
        }
    }

    private bool TryParseInput(out double testVoltage,
                               out double ratedPrimaryVoltage,
                               out double ratedSecondaryVoltage,
                               out string errorMessage)
    {
        testVoltage           = 0;
        ratedPrimaryVoltage   = 0;
        ratedSecondaryVoltage = 0;
        errorMessage          = string.Empty;

        var culture = CultureInfo.InvariantCulture;

        if (!double.TryParse(TestVoltageTextBox.Text?.Trim(), NumberStyles.Float, culture, out testVoltage) ||
            !InputValidator.ValidateTestVoltage(testVoltage, out errorMessage))
        {
            if (string.IsNullOrEmpty(errorMessage))
                errorMessage = "Test voltage must be a positive number. Maximum allowed is 42 V.";
            return false;
        }

        if (!double.TryParse(RatedPrimaryVoltageTextBox.Text?.Trim(), NumberStyles.Float, culture, out ratedPrimaryVoltage) ||
            !double.TryParse(RatedSecondaryVoltageTextBox.Text?.Trim(), NumberStyles.Float, culture, out ratedSecondaryVoltage) ||
            !InputValidator.ValidateNameplateVoltages(ratedPrimaryVoltage, ratedSecondaryVoltage, out errorMessage))
        {
            if (string.IsNullOrEmpty(errorMessage))
                errorMessage = "Nameplate voltages must be positive numbers.";
            return false;
        }

        return true;
    }
}
