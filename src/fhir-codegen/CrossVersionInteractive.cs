// <copyright file="CrossVersionInteractive.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager;
using Terminal.Gui;

namespace fhir_codegen;

internal class CrossVersionInteractive
{
    private static CrossVersionInteractive _instance = null!;

    private ConfigCrossVersionInteractive _config;

    public static async Task<int> DoCrossVersionReview(System.CommandLine.Parsing.ParseResult pr)
    {
        ConfigCrossVersionInteractive config = new();
        config.Parse(pr);

        try
        {
            if (_instance is null)
            {
                _instance = new CrossVersionInteractive(config);
            }

            _instance.Run();
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.WriteLine($"DoCrossVersionReview <<< caught: {ex.Message}::{ex.InnerException.Message}");
            }
            else
            {
                Console.WriteLine($"DoCrossVersionReview <<< caught: {ex.Message}");
            }
        }
        finally
        {
            Application.Shutdown();
            await Task.Delay(0);
        }

        return 0;
    }

    private CrossVersionInteractive(ConfigCrossVersionInteractive config)
    {
        _config = config;
    }

    internal enum UiStateCodes
    {
        Unknown,
        Default,
        SelectPackages,
        LoadPackages,
        Compare,
        ProcessOverview,
        ProcessValueSets,
        ProcessPrimitiveTypes,
        ProcessComplexTypes,
        ProcessResources,
        Done,
    }

    private UiStateCodes _state = UiStateCodes.Default;

    private List<FhirReleases.PublishedReleaseInformation> _releases = FhirReleases.FhirPublishedVersions.Values.Where(i => i.IsSequenceOfficial).OrderBy(i => i.Sequence).ToList();

    private FhirReleases.PublishedReleaseInformation? _releaseLeft = null;
    private FhirReleases.PublishedReleaseInformation? _releaseRight = null;

    private DefinitionCollection? _dcLeft = null;
    private DefinitionCollection? _dcRight = null;

    private PackageComparer.PackageComparison? _comparison = null;

    private int _topY = 0;

    private System.Timers.Timer? _uiRefreshTimer = null;

    private void Run()
    {
        Application.Init();

        MenuBarItem menus = new MenuBarItem("_File", new MenuItem[] {
            new MenuItem ("_Quit", "", () => {
                _state = UiStateCodes.Done;
                Application.RequestStop();
            })
        });

        // loop until the user quits
        while ((_state != UiStateCodes.Done) && (_state != UiStateCodes.Unknown))
        {
            if (ConfigurationManager.Themes != null)
            {
                ConfigurationManager.Themes.Theme = "Default";
                ConfigurationManager.Apply();
            }

            Window mainWindow = new()
            {
                Title = "FHIR Cross Version Review (Ctrl+Q to quit)",
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            MenuBar menuBar = new MenuBar()
            {
                X = 0,
                Y = 0,
                Height = 1,
                Width = Dim.Fill(),
                CanFocus = false,
                Menus = [menus],
            };
            mainWindow.Add(menuBar);

            Window contentWindow = new()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            UiStateCodes current = _state;
            // reset state before running the next UI
            _state = UiStateCodes.Unknown;

            AddUiForState(current, contentWindow);
            mainWindow.Add(contentWindow);
            //AddUiForState(current, mainWindow);

            Application.Run(mainWindow);
            mainWindow.Dispose();
            Application.Shutdown();
        }

        _uiRefreshTimer?.Stop();
        _uiRefreshTimer?.Dispose();
    }


    private void AddUiForState(UiStateCodes state, Window w)
    {
        switch (state)
        {
            case UiStateCodes.Default:
                {
                    AddDefaultUi(w);
                    _state = UiStateCodes.Default;
                }
                break;

            case UiStateCodes.SelectPackages:
                {
                    AddSelectPackageUi(w);
                    _state = (_releaseLeft is null || _releaseRight is null) ? UiStateCodes.Default : UiStateCodes.LoadPackages;
                }
                break;

            case UiStateCodes.LoadPackages:
                {
                    AddLoadPackageUi(w);
                    _state = UiStateCodes.LoadPackages;
                }
                break;

            case UiStateCodes.Compare:
                {
                    AddCompareUi(w);
                    _state = UiStateCodes.Compare;
                }
                break;

            case UiStateCodes.Done:
                {
                    _state = UiStateCodes.Done;
                }
                break;
        }
    }

    private void AddProcessOverviewUi(Window w)
    {

    }


    private (Label labelLeft, Label labelRight) AddLeftAndRightLabels(Window w)
    {
        Label labelLeft = new()
        {
            X = 0,
            Y = _topY,
            Width = Dim.Percent(100),
            Height = 1,
            Text = $" Left package: hl7.fhir.{_releaseLeft?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_releaseLeft?.Version}",
        };
        w.Add(labelLeft);

        Label labelRight = new()
        {
            X = 0,
            Y = Pos.Bottom(labelLeft),
            Width = Dim.Percent(100),
            Height = 1,
            Text = $"Right package: hl7.fhir.{_releaseRight?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_releaseRight?.Version}",
        };
        w.Add(labelRight);

        return (labelLeft, labelRight);
    }

    private void AddDefaultUi(Window w)
    {
        Button selectButton = new()
        {
            Text = "Select _Packages",
            X = 0,
            Y = _topY,
        };

        selectButton.Accept += (s, e) =>
        {
            _state = UiStateCodes.SelectPackages;
            Application.RequestStop();
        };

        w.Add(selectButton);
    }

    private void AddCompareUi(Window w)
    {
        (Label labelLeft, Label labelRight) = AddLeftAndRightLabels(w);

        Button buttonCompare = new()
        {
            X = 0,
            Y = Pos.Bottom(labelRight),
            Height = 1,
            Text = "_Compare Packages",
        };

        Label labelComparing = new()
        {
            X = 0,
            Y = Pos.Bottom(labelRight),
            Width = 12,
            Height = 1,
            Text = "Comparing...",
            Visible = false,
        };
        w.Add(labelComparing);

        SpinnerView spinnerView = new SpinnerView()
        {
            X = Pos.Right(labelComparing),
            Y = Pos.Top(labelComparing),
            Width = 10,
            Height = 1,
            AutoSpin = false,
            Visible = false,
        };
        w.Add(spinnerView);

        buttonCompare.Accept += (s, e) =>
        {
            buttonCompare.Visible = false;
            labelComparing.Visible = true;
            spinnerView.Visible = true;
            spinnerView.SpinDelay = 125;
            spinnerView.AutoSpin = true;

            w.SetNeedsDisplay();

            Task.Run(async () =>
            {
                (bool success, string message) = await TryCompareAsync();
                if (success)
                {
                    _state = UiStateCodes.ProcessOverview;
                    Application.RequestStop();
                }
                else
                {
                    _dcLeft = null;
                    _dcRight = null;
                    labelComparing.Visible = false;
                    spinnerView.AutoSpin = false;
                    spinnerView.Visible = false;
                    buttonCompare.Visible = true;

                    MessageBox.ErrorQuery(40, 7, "Error", message, "Ok");
                }
            });
        };

        w.Add(buttonCompare);
    }

    private void AddLoadPackageUi(Window w)
    {
        (Label labelLeft, Label labelRight) = AddLeftAndRightLabels(w);

        Button buttonLoad = new()
        {
            X = 0,
            Y = Pos.Bottom(labelRight),
            Height = 1,
            Text = "_Load Packages",
        };

        Label labelLoading = new()
        {
            X = 0,
            Y = Pos.Bottom(labelRight),
            Width = 10,
            Height = 1,
            Text = "Loading...",
            Visible = false,
        };
        w.Add(labelLoading);

        SpinnerView spinnerView = new SpinnerView()
        {
            X = Pos.Right(labelLoading),
            Y = Pos.Top(labelLoading),
            Width = 10,
            Height = 1,
            AutoSpin = false,
            //SpinDelay = 125,
            //SpinBounce = true,
            Visible = false,
            //Style = new SpinnerStyle.Points(),
        };
        w.Add(spinnerView);

        buttonLoad.Accept += (s, e) =>
        {
            buttonLoad.Visible = false;
            labelLoading.Visible = true;
            spinnerView.Visible = true;
            spinnerView.SpinDelay = 125;
            spinnerView.AutoSpin = true;

            //_uiRefreshTimer = new()
            //{
            //    AutoReset = true,
            //    Interval = 150,
            //};
            //_uiRefreshTimer.Elapsed += (s, e) => { spinnerView. };

            w.SetNeedsDisplay();

            Task.Run(async () =>
            {
                (bool success, string message) = await TryLoadPackagesAsync();
                if (success)
                {
                    _state = UiStateCodes.Compare;
                    Application.RequestStop();
                }
                else
                {
                    _dcLeft = null;
                    _dcRight = null;
                    labelLoading.Visible = false;
                    spinnerView.AutoSpin = false;
                    spinnerView.Visible = false;
                    buttonLoad.Visible = true;

                    MessageBox.ErrorQuery(40, 7, "Error", message, "Ok");
                }
            });

            //_state = UiStateCodes.LoadingPackages;
            //Application.RequestStop();
        };

        w.Add(buttonLoad);
    }

    private void AddSelectPackageUi(Window w)
    {
        Button cancelButton = new()
        {
            Text = "_Cancel",
            //X = 0,
            //Y = 0,
        };

        Button okButton = new()
        {
            Text = "_OK",
        };

        Dialog d = new()
        {
            Title = "Select Packages...",
            Buttons = [cancelButton, okButton],
            X = 0,
            Y = _topY,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ButtonAlignment = Dialog.ButtonAlignments.Center,
        };

        cancelButton.Accept += (s, e) =>
        {
            _releaseLeft = null;
            _releaseRight = null;
            Application.RequestStop(d);
        };

        ListView selectLeft = new()
        {
            X = 0,
            //Y = Pos.Bottom(labelSelectedLeft) + 1,
            Y = 0,
            Width = Dim.Percent(50),
            Height = Dim.Fill(1),
            AllowsMarking = true,
            AllowsMultipleSelection = false,
        };
        selectLeft.SetSource(_releases);
        d.Add(selectLeft);

        ListView selectRight = new()
        {
            X = Pos.Right(selectLeft) + 1,
            //Y = Pos.Bottom(labelSelectedRight) + 1,
            Y = 0,
            Width = Dim.Percent(50),
            Height = Dim.Fill(1),
            AllowsMarking = true,
            AllowsMultipleSelection = false,
        };
        selectRight.SetSource(_releases);
        d.Add(selectRight);

        okButton.Accept += (s, e) =>
        {
            for(int i = 0; i < selectLeft.Source.Count; i++)
            {
                if (selectLeft.Source.IsMarked(i))
                {
                    _releaseLeft = _releases[i];
                    break;
                }
            }

            for (int i = 0; i < selectRight.Source.Count; i++)
            {
                if (selectRight.Source.IsMarked(i))
                {
                    _releaseRight = _releases[i];
                    break;
                }
            }

            Application.RequestStop(d);
        };

        w.Add(d);
    }

    private async Task<(bool success, string message)> TryCompareAsync()
    {
        if ((_dcLeft is null) || (_dcRight is null))
        {
            return (false, "Two loaded FHIR packages are required.");
        }

        await Task.Delay(0);

        try
        {
            ConfigCompare compareConfig = new()
            {
                FhirCacheDirectory = _config.FhirCacheDirectory,
                NoOutput = true,
            };

            PackageComparer comparer = new(compareConfig, _dcLeft, _dcRight);

            _comparison = comparer.Compare();

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryCompareAsync <<< caught: {ex.Message}");
            return (false, ex.Message + (ex.InnerException is null ? string.Empty : ": " + ex.InnerException.Message));
        }
    }


    private async Task<(bool success, string message)> TryLoadPackagesAsync()
    {
        if ((_releaseLeft is null) || (_releaseRight is null))
        {
            return (false, "Two FHIR packages are required.");
        }

        try
        {
            // create our cache object to load packages with
            IFhirPackageClient cache = FhirCache.Create(new FhirPackageClientSettings()
            {
                CachePath = _config.FhirCacheDirectory,
            });

            string directiveLeft = $"hl7.fhir.{_releaseLeft.Value.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_releaseLeft.Value.Version}";
            PackageCacheEntry? packageLeft = await cache.FindOrDownloadPackageByDirective(directiveLeft, true);

            if (packageLeft is null)
            {
                return (false, $"Could not find package for directive: {directiveLeft}");
            }

            string directiveRight = $"hl7.fhir.{_releaseRight.Value.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_releaseRight.Value.Version}";
            PackageCacheEntry? packageRight = await cache.FindOrDownloadPackageByDirective(directiveRight, true);

            if (packageRight is null)
            {
                return (false, $"Could not find package for directive: {directiveRight}");
            }

            PackageLoader loaderLeft = new(cache, new()
            {
                JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
                AutoLoadExpansions = true,
                ResolvePackageDependencies = true,
            });

            _dcLeft = loaderLeft.LoadPackages(packageLeft.Name, [ packageLeft ])
                ?? throw new Exception($"Could not load left-hand-side package: {directiveLeft}");

            PackageLoader loaderRight = new(cache, new()
            {
                JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
                AutoLoadExpansions = true,
                ResolvePackageDependencies = true,
            });

            _dcRight = loaderRight.LoadPackages(packageRight.Name, [ packageRight ])
                ?? throw new Exception($"Could not load right-hand-side package: {directiveRight}");

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryLoadPackagesAsync <<< caught: {ex.Message}");
            return (false, ex.Message + (ex.InnerException is null ? string.Empty : ": " + ex.InnerException.Message));
        }
    }

}
