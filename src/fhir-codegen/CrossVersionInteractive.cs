// <copyright file="CrossVersionInteractive.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.CodeGen.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
//using Terminal.Gui;
using static Microsoft.Health.Fhir.CodeGen.CompareTool.PackageComparer;

namespace fhir_codegen;

internal class CrossVersionInteractive
{
    //private static CrossVersionInteractive _instance = null!;

    //private ConfigCrossVersionInteractive _config;

    //public static async Task<int> DoCrossVersionReview(System.CommandLine.Parsing.ParseResult pr)
    //{
    //    ConfigCrossVersionInteractive config = new();
    //    config.Parse(pr);

    //    try
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = new CrossVersionInteractive(config);
    //        }

    //        _instance.Run();
    //    }
    //    catch (Exception ex)
    //    {
    //        if (ex.InnerException != null)
    //        {
    //            Console.WriteLine($"DoCrossVersionReview <<< caught: {ex.Message}::{ex.InnerException.Message}");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"DoCrossVersionReview <<< caught: {ex.Message}");
    //        }
    //    }
    //    finally
    //    {
    //        Application.Shutdown();
    //        await Task.Delay(0);
    //    }

    //    return 0;
    //}

    //private CrossVersionInteractive(ConfigCrossVersionInteractive config)
    //{
    //    _config = config;

    //    // check for existing package selection
    //    if (!string.IsNullOrEmpty(config.LeftPackageDirective))
    //    {
    //        if (FhirReleases.TryGetSequence(config.LeftPackageDirective.Split('#').First(), out FhirReleases.FhirSequenceCodes leftSeq))
    //        {
    //            _releaseLeft = _releases.Where(i => i.Sequence == leftSeq).FirstOrDefault();
    //        }
    //    }

    //    if (!string.IsNullOrEmpty(config.RightPackageDirective))
    //    {
    //        if (FhirReleases.TryGetSequence(config.RightPackageDirective.Split('#').First(), out FhirReleases.FhirSequenceCodes rightSeq))
    //        {
    //            _releaseRight = _releases.Where(i => i.Sequence == rightSeq).FirstOrDefault();
    //        }
    //    }

    //    if ((_releaseLeft != null) && (_releaseRight != null))
    //    {
    //        // jump to loading packages
    //        _nextState = UiStateCodes.LoadPackages;
    //    }

    //    if (!string.IsNullOrEmpty(config.ExistingComparisonPath))
    //    {
    //        string filename = config.ExistingComparisonPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
    //            ? config.ExistingComparisonPath
    //            : Path.Combine(config.ExistingComparisonPath, "comparison.json");

    //        if (TryLoadExistingComparison(filename))
    //        {
    //            // jump to process overview
    //            _nextState = UiStateCodes.ProcessOverview;
    //        }
    //    }
    //}

    //private bool TryLoadExistingComparison(string filename)
    //{
    //    if (File.Exists(filename))
    //    {
    //        using FileStream jsonFs = new(filename, FileMode.Open, FileAccess.Read);
    //        {
    //            try
    //            {
    //                _comparison = JsonSerializer.Deserialize<PackageComparison>(jsonFs);

    //                // grab package information from comparison
    //                if ((_comparison != null) &&
    //                    FhirReleases.TryGetSequence(_comparison.LeftPackageId, out FhirReleases.FhirSequenceCodes leftSeq))
    //                {
    //                    _releaseLeft = _releases.Where(i => i.Sequence == leftSeq).FirstOrDefault();
    //                }

    //                if ((_comparison != null) &&
    //                    FhirReleases.TryGetSequence(_comparison.RightPackageId, out FhirReleases.FhirSequenceCodes rightSeq))
    //                {
    //                    _releaseRight = _releases.Where(i => i.Sequence == rightSeq).FirstOrDefault();
    //                }

    //                return true;
    //            }
    //            finally
    //            {
    //                jsonFs.Close();
    //            }
    //        }
    //    }

    //    return false;
    //}

    //internal enum UiStateCodes
    //{
    //    Unknown,
    //    Default,
    //    SelectExisting,
    //    SelectPackages,
    //    LoadPackages,
    //    Compare,
    //    ProcessOverview,
    //    ProcessValueSets,
    //    ProcessPrimitiveTypes,
    //    ProcessComplexTypes,
    //    ProcessResources,
    //    Done,
    //}

    //private UiStateCodes _nextState = UiStateCodes.Default;

    //private List<FhirReleases.PublishedReleaseInformation> _releases = FhirReleases.FhirPublishedVersions.Values.Where(i => i.IsSequenceOfficial).OrderBy(i => i.Sequence).ToList();

    //private FhirReleases.PublishedReleaseInformation? _releaseLeft = null;
    //private FhirReleases.PublishedReleaseInformation? _releaseRight = null;

    //private DefinitionCollection? _dcLeft = null;
    //private DefinitionCollection? _dcRight = null;

    //private PackageComparison? _comparison = null;

    //private int _topY = 0;

    //private System.Timers.Timer? _uiRefreshTimer = null;

    //private void Run()
    //{
    //    Application.Init();
    //    //Application.QuitKey = Key.Q.WithCtrl;

    //    MenuBarItem menus = new MenuBarItem("_File", new MenuItem[] {
    //        new MenuItem ("_Quit", "", () => {
    //            _nextState = UiStateCodes.Done;
    //            Application.RequestStop();
    //        })
    //    });

    //    // loop until the user quits
    //    while ((_nextState != UiStateCodes.Done) && (_nextState != UiStateCodes.Unknown))
    //    {
    //        if (Terminal.Gui.ConfigurationManager.Themes != null)
    //        {
    //            Terminal.Gui.ConfigurationManager.Themes.Theme = "Default";
    //            Terminal.Gui.ConfigurationManager.Apply();
    //        }

    //        Window mainWindow = new()
    //        {
    //            Title = "FHIR Cross Version Review (Ctrl+Q to quit)",
    //            X = 0,
    //            Y = 0,
    //            Width = Dim.Fill(),
    //            Height = Dim.Fill(),
    //        };

    //        MenuBar menuBar = new MenuBar()
    //        {
    //            X = 0,
    //            Y = 0,
    //            Height = 1,
    //            Width = Dim.Fill(),
    //            CanFocus = false,
    //            Menus = [menus],
    //        };
    //        mainWindow.Add(menuBar);

    //        Window contentWindow = new()
    //        {
    //            X = 0,
    //            Y = 1,
    //            Width = Dim.Fill(),
    //            Height = Dim.Fill(),
    //        };

    //        UiStateCodes current = _nextState;
    //        // reset state before running the next UI
    //        _nextState = UiStateCodes.Unknown;

    //        AddUiForState(current, contentWindow);
    //        mainWindow.Add(contentWindow);

    //        //AddUiForState(current, mainWindow);

    //        Application.Run(mainWindow);
    //        mainWindow.Dispose();
    //        Application.Shutdown();
    //    }

    //    _uiRefreshTimer?.Stop();
    //    _uiRefreshTimer?.Dispose();
    //}


    //private void AddUiForState(UiStateCodes state, Window w)
    //{
    //    switch (state)
    //    {
    //        case UiStateCodes.Default:
    //            {
    //                AddDefaultUi(w);
    //            }
    //            break;

    //        case UiStateCodes.SelectExisting:
    //            {
    //                throw new NotImplementedException();
    //            }
    //            //break;

    //        case UiStateCodes.SelectPackages:
    //            {
    //                AddSelectPackageUi(w);
    //            }
    //            break;

    //        case UiStateCodes.LoadPackages:
    //            {
    //                AddLoadPackageUi(w);
    //            }
    //            break;

    //        case UiStateCodes.Compare:
    //            {
    //                AddCompareUi(w);
    //            }
    //            break;

    //        case UiStateCodes.ProcessOverview:
    //            {
    //                AddProcessOverviewUi(w);
    //            }
    //            break;

    //        case UiStateCodes.ProcessValueSets:
    //            {
    //                //_nextState = UiStateCodes.ProcessValueSets;
    //            }
    //            break;

    //        case UiStateCodes.ProcessPrimitiveTypes:
    //            {
    //                //_nextState = UiStateCodes.ProcessPrimitiveTypes;
    //            }
    //            break;

    //        case UiStateCodes.ProcessComplexTypes:
    //            {
    //                //_nextState = UiStateCodes.ProcessComplexTypes;
    //            }
    //            break;

    //        case UiStateCodes.ProcessResources:
    //            {
    //                //_nextState = UiStateCodes.ProcessResources;
    //            }
    //            break;

    //        case UiStateCodes.Done:
    //            {
    //                //_nextState = UiStateCodes.Done;
    //            }
    //            break;
    //    }
    //}

    //private void AddProcessOverviewUi(Window w)
    //{
    //    (Label labelLeft, Label labelRight) = AddLeftAndRightLabels(w);

    //    Label labelVs = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelRight) + 1,
    //        Width = 20,
    //        Height = 1,
    //        Text = "   Value Sets: " + _comparison?.ValueSets.Count,
    //    };
    //    w.Add(labelVs);

    //    Button buttonVs = new()
    //    {
    //        X = 21,
    //        Y = Pos.Top(labelVs),
    //        Height = 1,
    //        Text = "Process _Value Sets",
    //        HotKey = Key.V,
    //    };
    //    buttonVs.Accept += (s, e) =>
    //    {
    //        _nextState = UiStateCodes.ProcessValueSets;
    //        Application.RequestStop();
    //    };
    //    w.Add(buttonVs);

    //    Label labelPrimitives = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelVs),
    //        Width = 20,
    //        Height = 1,
    //        Text = "   Primitives: " + _comparison?.PrimitiveTypes.Count,
    //    };
    //    w.Add(labelPrimitives);

    //    Button buttonPrimitives = new()
    //    {
    //        X = 21,
    //        Y = Pos.Top(labelPrimitives),
    //        Height = 1,
    //        Text = "Process _Primitives",
    //        HotKey = Key.P,
    //    };
    //    buttonPrimitives.Accept += (s, e) =>
    //    {
    //        _nextState = UiStateCodes.ProcessPrimitiveTypes;
    //        Application.RequestStop();
    //    };
    //    w.Add(buttonPrimitives);

    //    Label labelComplexTypes = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelPrimitives),
    //        Width = 20,
    //        Height = 1,
    //        Text = "Complex Types: " + _comparison?.ComplexTypes.Count,
    //    };
    //    w.Add(labelComplexTypes);

    //    Button buttonComplexTypes = new()
    //    {
    //        X = 21,
    //        Y = Pos.Top(labelComplexTypes),
    //        Height = 1,
    //        Text = "Process _Complex Types",
    //        HotKey = Key.C,
    //    };
    //    buttonComplexTypes.Accept += (s, e) =>
    //    {
    //        _nextState = UiStateCodes.ProcessComplexTypes;
    //        Application.RequestStop();
    //    };
    //    w.Add(buttonComplexTypes);

    //    Label labelResources = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelComplexTypes),
    //        Width = 20,
    //        Height = 1,
    //        Text = "    Resources: " + _comparison?.Resources.Count,
    //    };
    //    w.Add(labelResources);

    //    Button buttonResources = new()
    //    {
    //        X = 21,
    //        Y = Pos.Top(labelResources),
    //        Height = 1,
    //        Text = "Process _Resources",
    //        HotKey = Key.R,
    //    };
    //    buttonResources.Accept += (s, e) =>
    //    {
    //        _nextState = UiStateCodes.ProcessResources;
    //        Application.RequestStop();
    //    };
    //    w.Add(buttonResources);
    //}

    //private (Label labelLeft, Label labelRight) AddLeftAndRightLabels(Window w)
    //{
    //    Label labelLeft = new()
    //    {
    //        X = 0,
    //        Y = _topY,
    //        Width = Dim.Percent(100),
    //        Height = 1,
    //        Text = $" Left package: hl7.fhir.{_releaseLeft?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_releaseLeft?.Version}",
    //    };
    //    w.Add(labelLeft);

    //    Label labelRight = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelLeft),
    //        Width = Dim.Percent(100),
    //        Height = 1,
    //        Text = $"Right package: hl7.fhir.{_releaseRight?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_releaseRight?.Version}",
    //    };
    //    w.Add(labelRight);

    //    return (labelLeft, labelRight);
    //}

    //private void AddDefaultUi(Window w)
    //{
    //    Button selectPackagesButton = new()
    //    {
    //        Text = "Select _Packages",
    //        X = 0,
    //        Y = _topY,
    //        HotKey = Key.P,
    //    };

    //    selectPackagesButton.Accept += (s, e) =>
    //    {
    //        _nextState = UiStateCodes.SelectPackages;
    //        Application.RequestStop();
    //    };

    //    w.Add(selectPackagesButton);

    //    Button selectExistingButton = new()
    //    {
    //        Text = "Select _Existing",
    //        X = 0,
    //        Y = _topY,
    //        HotKey = Key.E,
    //    };

    //    selectExistingButton.Accept += (s, e) =>
    //    {
    //        _nextState = UiStateCodes.SelectExisting;
    //        Application.RequestStop();
    //    };

    //    w.Add(selectExistingButton);
    //}

    //private void AddCompareUi(Window w)
    //{
    //    (Label labelLeft, Label labelRight) = AddLeftAndRightLabels(w);

    //    Button buttonCompare = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelRight),
    //        Height = 1,
    //        Text = "_Compare Packages",
    //    };

    //    Label labelComparing = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelRight),
    //        Width = 12,
    //        Height = 1,
    //        Text = "Comparing...",
    //        Visible = false,
    //    };
    //    w.Add(labelComparing);

    //    SpinnerView spinnerView = new SpinnerView()
    //    {
    //        X = Pos.Right(labelComparing),
    //        Y = Pos.Top(labelComparing),
    //        Width = 10,
    //        Height = 1,
    //        AutoSpin = false,
    //        Visible = false,
    //    };
    //    w.Add(spinnerView);

    //    buttonCompare.Accept += (s, e) =>
    //    {
    //        buttonCompare.Visible = false;
    //        labelComparing.Visible = true;
    //        spinnerView.Visible = true;
    //        spinnerView.SpinDelay = 125;
    //        spinnerView.AutoSpin = true;

    //        w.SetNeedsDisplay();

    //        Task.Run(async () =>
    //        {
    //            (bool success, string message) = await TryCompareAsync();
    //            if (success)
    //            {
    //                _nextState = UiStateCodes.ProcessOverview;
    //            }
    //            else
    //            {
    //                _dcLeft = null;
    //                _dcRight = null;
    //                labelComparing.Visible = false;
    //                spinnerView.AutoSpin = false;
    //                spinnerView.Visible = false;
    //                buttonCompare.Visible = true;

    //                MessageBox.ErrorQuery(40, 7, "Error", message, "Ok");
    //                _nextState = UiStateCodes.Default;
    //            }

    //            Application.RequestStop();
    //        });
    //    };

    //    w.Add(buttonCompare);
    //}

    //private void AddLoadPackageUi(Window w)
    //{
    //    (Label labelLeft, Label labelRight) = AddLeftAndRightLabels(w);

    //    Button buttonLoad = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelRight),
    //        Height = 1,
    //        Text = "_Load Packages",
    //    };

    //    Label labelLoading = new()
    //    {
    //        X = 0,
    //        Y = Pos.Bottom(labelRight),
    //        Width = 10,
    //        Height = 1,
    //        Text = "Loading...",
    //        Visible = false,
    //    };
    //    w.Add(labelLoading);

    //    SpinnerView spinnerView = new SpinnerView()
    //    {
    //        X = Pos.Right(labelLoading),
    //        Y = Pos.Top(labelLoading),
    //        Width = 10,
    //        Height = 1,
    //        AutoSpin = false,
    //        //SpinDelay = 125,
    //        //SpinBounce = true,
    //        Visible = false,
    //        //Style = new SpinnerStyle.Points(),
    //    };
    //    w.Add(spinnerView);

    //    buttonLoad.Accept += (s, e) =>
    //    {
    //        buttonLoad.Visible = false;
    //        labelLoading.Visible = true;
    //        spinnerView.Visible = true;
    //        spinnerView.SpinDelay = 125;
    //        spinnerView.AutoSpin = true;

    //        //_uiRefreshTimer = new()
    //        //{
    //        //    AutoReset = true,
    //        //    Interval = 150,
    //        //};
    //        //_uiRefreshTimer.Elapsed += (s, e) => { spinnerView. };

    //        w.SetNeedsDisplay();

    //        Task.Run(async () =>
    //        {
    //            (bool success, string message) = await TryLoadPackagesAsync();
    //            if (success)
    //            {
    //                _nextState = UiStateCodes.Compare;
    //            }
    //            else
    //            {
    //                _dcLeft = null;
    //                _dcRight = null;
    //                labelLoading.Visible = false;
    //                spinnerView.AutoSpin = false;
    //                spinnerView.Visible = false;
    //                buttonLoad.Visible = true;

    //                MessageBox.ErrorQuery(40, 7, "Error", message, "Ok");
    //                _nextState = UiStateCodes.Default;
    //            }

    //            Application.RequestStop();
    //        });
    //    };

    //    w.Add(buttonLoad);
    //}

    //private void AddSelectPackageUi(Window w)
    //{
    //    Button cancelButton = new()
    //    {
    //        Text = "_Cancel",
    //        //X = 0,
    //        //Y = 0,
    //    };

    //    Button okButton = new()
    //    {
    //        Text = "_OK",
    //    };

    //    Dialog d = new()
    //    {
    //        Title = "Select Packages...",
    //        Buttons = [cancelButton, okButton],
    //        X = 0,
    //        Y = _topY,
    //        Width = Dim.Fill(),
    //        Height = Dim.Fill(),
    //        ButtonAlignment = Dialog.ButtonAlignments.Center,
    //    };

    //    cancelButton.Accept += (s, e) =>
    //    {
    //        _releaseLeft = null;
    //        _releaseRight = null;
    //        _nextState = UiStateCodes.Default;
    //        Application.RequestStop(d);
    //    };

    //    ListView selectLeft = new()
    //    {
    //        X = 0,
    //        //Y = Pos.Bottom(labelSelectedLeft) + 1,
    //        Y = 0,
    //        Width = Dim.Percent(50),
    //        Height = Dim.Fill(1),
    //        AllowsMarking = true,
    //        AllowsMultipleSelection = false,
    //    };
    //    selectLeft.SetSource(_releases);
    //    d.Add(selectLeft);

    //    ListView selectRight = new()
    //    {
    //        X = Pos.Right(selectLeft) + 1,
    //        //Y = Pos.Bottom(labelSelectedRight) + 1,
    //        Y = 0,
    //        Width = Dim.Percent(50),
    //        Height = Dim.Fill(1),
    //        AllowsMarking = true,
    //        AllowsMultipleSelection = false,
    //    };
    //    selectRight.SetSource(_releases);
    //    d.Add(selectRight);

    //    okButton.Accept += (s, e) =>
    //    {
    //        for(int i = 0; i < selectLeft.Source.Count; i++)
    //        {
    //            if (selectLeft.Source.IsMarked(i))
    //            {
    //                _releaseLeft = _releases[i];
    //                break;
    //            }
    //        }

    //        for (int i = 0; i < selectRight.Source.Count; i++)
    //        {
    //            if (selectRight.Source.IsMarked(i))
    //            {
    //                _releaseRight = _releases[i];
    //                break;
    //            }
    //        }

    //        _nextState = (_releaseLeft == null || _releaseRight == null) ? UiStateCodes.Default : UiStateCodes.LoadPackages;

    //        Application.RequestStop(d);
    //    };

    //    w.Add(d);
    //}

    //private async Task<(bool success, string message)> TryCompareAsync()
    //{
    //    if ((_dcLeft == null) || (_dcRight == null))
    //    {
    //        return (false, "Two loaded FHIR packages are required.");
    //    }

    //    await Task.Delay(0);

    //    try
    //    {
    //        ConfigCompare compareConfig = new()
    //        {
    //            FhirCacheDirectory = _config.FhirCacheDirectory,
    //            NoOutput = true,
    //        };

    //        IFhirPackageClient cache = FhirCache.Create(new FhirPackageClientSettings()
    //        {
    //            CachePath = _config.FhirCacheDirectory,
    //        });

    //        PackageComparer comparer = new(compareConfig, cache, _dcLeft, _dcRight);

    //        _comparison = comparer.Compare();

    //        return (true, string.Empty);
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"TryCompareAsync <<< caught: {ex.Message}");
    //        return (false, ex.Message + (ex.InnerException == null ? string.Empty : ": " + ex.InnerException.Message));
    //    }
    //}


    //private async Task<(bool success, string message)> TryLoadPackagesAsync()
    //{
    //    if ((_releaseLeft == null) || (_releaseRight == null))
    //    {
    //        return (false, "Two FHIR packages are required.");
    //    }

    //    try
    //    {
    //        // create our cache object to load packages with
    //        IFhirPackageClient cache = FhirCache.Create(new FhirPackageClientSettings()
    //        {
    //            CachePath = _config.FhirCacheDirectory,
    //        });

    //        string directiveLeft = $"hl7.fhir.{_releaseLeft.Value.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_releaseLeft.Value.Version}";
    //        PackageCacheEntry? packageLeft = await cache.FindOrDownloadPackageByDirective(directiveLeft, true);

    //        if (packageLeft == null)
    //        {
    //            return (false, $"Could not find package for directive: {directiveLeft}");
    //        }

    //        string directiveRight = $"hl7.fhir.{_releaseRight.Value.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_releaseRight.Value.Version}";
    //        PackageCacheEntry? packageRight = await cache.FindOrDownloadPackageByDirective(directiveRight, true);

    //        if (packageRight == null)
    //        {
    //            return (false, $"Could not find package for directive: {directiveRight}");
    //        }

    //        PackageLoader loaderLeft = new(cache, new()
    //        {
    //            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
    //            AutoLoadExpansions = true,
    //            ResolvePackageDependencies = true,
    //        });

    //        _dcLeft = loaderLeft.LoadPackages(packageLeft.Name, [ packageLeft ])
    //            ?? throw new Exception($"Could not load left-hand-side package: {directiveLeft}");

    //        PackageLoader loaderRight = new(cache, new()
    //        {
    //            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
    //            AutoLoadExpansions = true,
    //            ResolvePackageDependencies = true,
    //        });

    //        _dcRight = loaderRight.LoadPackages(packageRight.Name, [ packageRight ])
    //            ?? throw new Exception($"Could not load right-hand-side package: {directiveRight}");

    //        return (true, string.Empty);
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"TryLoadPackagesAsync <<< caught: {ex.Message}");
    //        return (false, ex.Message + (ex.InnerException == null ? string.Empty : ": " + ex.InnerException.Message));
    //    }
    //}

}
