// <copyright file="XVerHomeViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Material.Icons;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Loader;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.Comparison.XVer;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace fhir_codegen_app.ViewModels;

public partial class XVerHomeViewModel : ViewModelBase, INavigableViewModel
{
    public static string Label => "Cross Version Comparison";
    public static MaterialIconKind IconKind => MaterialIconKind.Compare;
    public static bool Indented => false;

    [ObservableProperty]
    private string _header = "FHIR Cross Version Comparison";

    [ObservableProperty]
    private string? _message = null;

    [ObservableProperty]
    private bool _busy = false;

    [ObservableProperty]
    private string _crossVersionDirectory = "git/fhir-cross-version";

    private ICodeGenConfig? _config;

    public XVerHomeViewModel() : this(null) { }

    public XVerHomeViewModel(object? args)
        : base()
    {
        _config = (args is ICodeGenConfig c)
            ? c
            : Ioc.Default.GetService<ICodeGenConfig>();

        // check to see if we have an XVer config already
        if (_config is ConfigXVer xvc)
        {
            Task.Run(() => doCrossVersionComparison(xvc));
        }
    }

    private void doCrossVersionComparison(ConfigXVer xvc)
    {
        Busy = true;
        Message = "Starting cross version comparison...";

        bool complete = false;
        
        try
        {
            //XVerProcessor xVerProcessor = new(xvc);
            //xVerProcessor.Compare();

            complete = true;
        }
        catch (Exception ex)
        {
            Message = $"Error: {ex.Message}";
        }

        if (complete)
        {
            Message = "Cross version comparison complete.";
        }

        Busy = false;
    }
}
