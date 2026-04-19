# RatioTestApp — Transformer Ratio Test

This project was completed as part of a Taster Day assignment.

## Overview

RatioTestApp is a WPF desktop application that was provided as a coding example. The task was to review the code, identify issues, and implement automated tests.

---

## Project Structure

```
RatioTestApp/
├── RatioTestApp.sln
├── .gitignore
├── README.md
├── RatioTestApp/                        # Main WPF application
│   ├── RatioTestApp.csproj
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── AssemblyInfo.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── IMeasurementDevice.cs
│   ├── SimulatedMeasurementDevice.cs
│   ├── RatioTest.cs
│   ├── RatioTestResult.cs
│   └── InputValidator.cs
└── RatioTestApp.Tests/                  # xUnit test project
    ├── RatioTestApp.Tests.csproj
    └── RatioTestTests.cs
```

---

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download) (net10.0)
- [Visual Studio Code](https://code.visualstudio.com/) with the XAML Form GUI Editor extension, or Visual Studio 2022

---

## Code Review Findings & Changes

### Bug fixes

**1. `VoltageRatio` was inverted** — `RatioTest.cs`

**2. `RatioDeviation` formula was wrong** — `RatioTest.cs`

**3. `MessageBox.Show()` inside business logic** — `RatioTest.cs`

**4. `TryParseInput` returned `true` on invalid primary voltage** — `MainWindow.xaml.cs`

**5. `ExpectedVoltageRatioTextBlock` not cleared on new run** — `MainWindow.xaml.cs`

**6. Duplicate `AutomationId` on `ExpectedVoltageRatioTextBlock`** — `MainWindow.xaml`

### Refactors

**7. Extracted `IMeasurementDevice` interface + `SimulatedMeasurementDevice`**

**8. Extracted `InputValidator` class**

**9. Split `RatioTest.cs` into one file per type**

**10. Added `RatioTestApp.Tests` project**