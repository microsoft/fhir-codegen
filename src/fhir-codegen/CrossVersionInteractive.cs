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
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Terminal.Gui;

namespace fhir_codegen;

internal class CrossVersionInteractive
{
    private static CrossVersionInteractive _instance = null!;

    public static async Task<int> DoCrossVersionReview(System.CommandLine.Parsing.ParseResult pr)
    {
        try
        {
            if (_instance is null)
            {
                _instance = new CrossVersionInteractive();
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

    internal enum UiStateCodes
    {
        Unknown,
        Main,
        SelectingPackages,
        LoadingPackages,
        Done,
    }

    private UiStateCodes _state = UiStateCodes.Main;

    private List<FhirReleases.PublishedReleaseInformation> _releases = FhirReleases.FhirPublishedVersions.Values.Where(i => i.IsSequenceOfficial).OrderBy(i => i.Sequence).ToList();

    private FhirReleases.PublishedReleaseInformation? _left = null;
    private FhirReleases.PublishedReleaseInformation? _right = null;

    private bool _packagesLoaded = false;

    private int _topY = 0;

    private System.Timers.Timer? _uiRefreshTimer = null;

    private void Run()
    {
        Application.Init();


        //MenuBar menu = new()
        //{
        //    Menus =
        //    [
        //        new MenuBarItem ("_File", new MenuItem [] {
        //            new MenuItem ("_Quit", "", () => {
        //                _state = UiStateCodes.Done;
        //                Application.RequestStop();
        //            })
        //        }),
        //    ]
        //};

        MenuBarItem menus = new MenuBarItem("_File", new MenuItem[] {
            new MenuItem ("_Quit", "", () => {
                _state = UiStateCodes.Done;
                Application.RequestStop();
            })
        });

        //Application.Top.Add(menu, mainWindow);

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
            case UiStateCodes.Main:
                {
                    AddMainUi(w);
                    _state = UiStateCodes.Main;
                }
                break;

            case UiStateCodes.SelectingPackages:
                {
                    _packagesLoaded = false;

                    try
                    {
                        w.Add(BuildSelectionUi());
                    }
                    catch (Exception)
                    {
                        // do nothing for now..
                    }

                    _state = (_left is null || _right is null) ? UiStateCodes.Main : UiStateCodes.LoadingPackages;
                }
                break;

            case UiStateCodes.LoadingPackages:
                {
                    AddLoadingUi(w);
                    _state = UiStateCodes.LoadingPackages;
                }
                break;
            case UiStateCodes.Done:
                {
                    _state = UiStateCodes.Done;
                }
                break;
        }
    }

    private void AddLoadingUi(Window w)
    {
        Label labelLeft = new()
        {
            X = 0,
            Y = _topY,
            Width = Dim.Percent(100),
            Height = 1,
            Text = $" Left package: hl7.fhir.{_left?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_left?.Version}",
        };
        w.Add(labelLeft);

        Label labelRight = new()
        {
            X = 0,
            Y = Pos.Bottom(labelLeft),
            Width = Dim.Percent(100),
            Height = 1,
            Text = $"Right package: hl7.fhir.{_right?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_right?.Version}",
        };
        w.Add(labelRight);

        Label labelLoading = new()
        {
            X = 0,
            Y = Pos.Bottom(labelRight) + 2,
            Width = 10,
            Height = 1,
            Text = "Loading..."
        };
        w.Add(labelLoading);

        SpinnerView spinnerView = new SpinnerView()
        {
            X = Pos.Right(labelLoading),
            Y = Pos.Top(labelLoading),
            Width = 10,
            Height = 1,
            AutoSpin = true,
            SpinDelay = 125,
            SpinBounce = true,
            Visible = true,
            Style = new SpinnerStyle.Points(),
        };
        w.Add(spinnerView);
        //spinnerView.AutoSpin = true;
    }

    private void AddMainUi(Window w)
    {
        if ((_left is null) || (_right is null))
        {
            Button selectButton = new()
            {
                Text = "Select _Packages",
                X = 0,
                Y = _topY,
            };

            selectButton.Accept += (s, e) =>
            {
                _state = UiStateCodes.SelectingPackages;
                Application.RequestStop();
            };

            w.Add(selectButton);

            return;
        }

        Label labelLeft = new()
        {
            X = 0,
            Y = _topY,
            Width = Dim.Percent(100),
            Height = 1,
            Text = $" Left package: hl7.fhir.{_left?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_left?.Version}",
        };
        w.Add(labelLeft);

        Label labelRight = new()
        {
            X = 0,
            Y = Pos.Bottom(labelLeft),
            Width = Dim.Percent(100),
            Height = 1,
            Text = $"Right package: hl7.fhir.{_right?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_right?.Version}",
        };
        w.Add(labelRight);

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
            SpinDelay = 125,
            SpinBounce = true,
            Visible = false,
            Style = new SpinnerStyle.Points(),
        };
        w.Add(spinnerView);

        buttonLoad.Accept += (s, e) =>
        {
            buttonLoad.Visible = false;
            labelLoading.Visible = true;
            spinnerView.Visible = true;

            //_uiRefreshTimer = new()
            //{
            //    AutoReset = true,
            //    Interval = 150,
            //};
            //_uiRefreshTimer.Elapsed += (s, e) => { spinnerView. };

            w.SetNeedsDisplay();

            //_state = UiStateCodes.LoadingPackages;
            //Application.RequestStop();
        };

        w.Add(buttonLoad);
    }

    private Dialog BuildSelectionUi()
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
            _left = null;
            _right = null;
            Application.RequestStop(d);
        };

        //d.Title = "Select packages for crossing...";

        //d.X = 0;
        //d.Y = 0;
        //d.Width = Dim.Fill();
        //d.Height = Dim.Fill();

        //Label labelSelectedLeft = new()
        //{
        //    X = 0,
        //    Y = 0,
        //    Width = Dim.Percent(50),
        //    Text = _left?.Description ?? "Select Left Version",
        //};
        //w.Add(labelSelectedLeft);

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
        //selectLeft.OpenSelectedItem += (s, e) =>
        //{
        //    //labelSelectedLeft.Text = _releases[e.Item].Description;
        //    //labelSelectedLeft.Draw();
        //    _left = _releases[e.Item];
        //    if ((_right is not null) && (_left is not null))
        //    {
        //        _state = UiStateCodes.Main;
        //        Application.RequestStop();
        //    }
        //};
        d.Add(selectLeft);

        //Label labelSelectedRight = new()
        //{
        //    X = Pos.Right(labelSelectedLeft) + 1,
        //    Y = 0,
        //    Width = Dim.Percent(50),
        //    Text = _right?.Description ?? "Select Right Version",
        //};
        //w.Add(labelSelectedRight);

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
        //selectRight.OpenSelectedItem += (s, e) =>
        //{
        //    //labelSelectedRight.Text = _releases[e.Item].Description;
        //    //labelSelectedRight.Draw();
        //    _right = _releases[e.Item];
        //    if ((_right is not null) && (_left is not null))
        //    {
        //        _state = UiStateCodes.Main;
        //        Application.RequestStop();
        //    }
        //};
        d.Add(selectRight);

        okButton.Accept += (s, e) =>
        {
            for(int i = 0; i < selectLeft.Source.Count; i++)
            {
                if (selectLeft.Source.IsMarked(i))
                {
                    _left = _releases[i];
                    break;
                }
            }

            for (int i = 0; i < selectRight.Source.Count; i++)
            {
                if (selectRight.Source.IsMarked(i))
                {
                    _right = _releases[i];
                    break;
                }
            }

            Application.RequestStop(d);
        };


        return d;
    }

}
