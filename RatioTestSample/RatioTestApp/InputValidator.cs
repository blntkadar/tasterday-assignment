using System.Globalization;

namespace RatioTestApp;

// REFACTOR: Validation logic extracted from the private TryParseInput method in
// MainWindow.xaml.cs into this dedicated class.
public static class InputValidator
{
    public static bool ValidateTestVoltage(double testVoltage, out string errorMessage)
    {
        if (testVoltage <= 0 || testVoltage > 42)
        {
            errorMessage = "Test voltage must be a positive number. Maximum allowed is 42 V.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    public static bool ValidateNameplateVoltages(double primaryVoltage, double secondaryVoltage, out string errorMessage)
    {
        if (primaryVoltage <= 0)
        {
            errorMessage = "Rated primary voltage must be a positive number.";
            return false;
        }

        if (secondaryVoltage <= 0)
        {
            errorMessage = "Rated secondary voltage must be a positive number.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
