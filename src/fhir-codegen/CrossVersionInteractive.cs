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

    private UiStateCodes _state = UiStateCodes.SelectingPackages;
    private bool _stateChanged = true;

    private List<FhirReleases.PublishedReleaseInformation> _releases = FhirReleases.FhirPublishedVersions.Values.Where(i => i.IsSequenceOfficial).OrderBy(i => i.Sequence).ToList();

    private FhirReleases.PublishedReleaseInformation? _left = null;
    private FhirReleases.PublishedReleaseInformation? _right = null;

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

            Application.Run(mainWindow);
            mainWindow.Dispose();
            Application.Shutdown();
        }
    }


    private void AddUiForState(UiStateCodes state, Window w)
    {
        switch (state)
        {
            case UiStateCodes.Main:
                {
                    if ((_left is null) || (_right is null))
                    {
                        Label label = new()
                        {
                            X = 0,
                            Y = 0,
                            Width = Dim.Percent(100),
                            Text = "Please select packages...",
                        };
                        w.Add(label);

                        Button selectButton = new()
                        {
                            Text = "_Select Packages",
                            X = 0,
                            Y = Pos.Bottom(label) + 1,
                        };

                        selectButton.Accept += (s, e) =>
                        {
                            _state = UiStateCodes.SelectingPackages;
                            Application.RequestStop();
                        };

                        //selectButton.Accept += (CancelEventArgs e) =>
                        //{
                        //    _state = UiStateCodes.SelectingPackages;
                        //    Application.RequestStop();
                        //};
                        w.Add(selectButton);

                        return;
                    }

                    Label labelLeft = new()
                    {
                        X = 0,
                        Y = 0,
                        Width = Dim.Percent(100),
                        Text = $" Left package: hl7.fhir.{_left?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_left?.Version}",
                    };
                    w.Add(labelLeft);

                    Label labelRight = new()
                    {
                        X = 0,
                        Y = Pos.Bottom(labelLeft) + 1,
                        Width = Dim.Percent(100),
                        Text = $"Right package: hl7.fhir.{_right?.Sequence.ToRLiteral().ToLowerInvariant()}.core#{_right?.Version}",
                    };
                    w.Add(labelRight);

                    _state = UiStateCodes.Main;
                }
                break;

            case UiStateCodes.SelectingPackages:
                {
                    try
                    {
                        Button cancelButton = new()
                        {
                            Text = "_Cancel",
                            X = 0,
                            Y = 0,
                        };

                        Dialog d = new()
                        {
                            Title = "Select Packages...",
                            Buttons = [ cancelButton ],
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

                        ConfigureSelectionUi(d);

                        w.Add(d);
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
                    //VersionSelectionDialog win = new();
                    //Application.Top.Add(menu, win);
                    //Application.Run();
                    //Application.Shutdown();

                    //_state = UiStateCodes.Done;

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

    private void ConfigureSelectionUi(Window w)
    {
        w.Title = "Select packages for crossing...";

        w.X = 0;
        w.Y = 0;
        w.Width = Dim.Fill();
        w.Height = Dim.Fill();

        Label labelSelectedLeft = new()
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(50),
            Text = _left?.Description ?? "Select Left Version",
        };
        w.Add(labelSelectedLeft);

        ListView selectLeft = new()
        {
            X = 0,
            Y = Pos.Bottom(labelSelectedLeft) + 1,
            Width = Dim.Percent(50),
            Height = Dim.Fill(1),
        };
        selectLeft.SetSource(_releases);
        w.Add(selectLeft);


        selectLeft.OpenSelectedItem += (s, e) =>
        {
            labelSelectedLeft.Text = _releases[e.Item].Description;
            _left = _releases[e.Item];
            if ((_right is not null) && (_left is not null))
            {
                _state = UiStateCodes.Main;
                Application.RequestStop();
            }
        };

        Label labelSelectedRight = new()
        {
            X = Pos.Right(labelSelectedLeft) + 1,
            Y = 0,
            Width = Dim.Percent(50),
            Text = _right?.Description ?? "Select Right Version",
        };
        w.Add(labelSelectedRight);

        ListView selectRight = new()
        {
            X = Pos.Right(selectLeft) + 1,
            Y = Pos.Bottom(labelSelectedRight) + 1,
            Width = Dim.Percent(50),
            Height = Dim.Fill(1),
        };
        selectRight.SetSource(_releases);
        w.Add(selectRight);

        selectRight.OpenSelectedItem += (s, e) =>
        {
            labelSelectedRight.Text = _releases[e.Item].Description;
            _right = _releases[e.Item];
            if ((_right is not null) && (_left is not null))
            {
                _state = UiStateCodes.Main;
                Application.RequestStop();
            }
        };
    }

}
