// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Diagnostics;
using System.IO;

namespace FhirCodegenTestCli
{
    /// <summary>The FHIR CodeGen Test CLI</summary>
    public static class Program
    {
        private const string CodegenCsproj = "src/fhir-codegen-cli/fhir-codegen-cli.csproj";

        /// <summary>The FHIR version minimum.</summary>
        private const int FhirVersionMin = 2;

        /// <summary>The FHIR version maximum.</summary>
        private const int FhirVersionMax = 5;

        /// <summary>True to verbose.</summary>
        private static bool _verbose = false;

        /// <summary>The tests run.</summary>
        private static int _testsRun = 0;

        /// <summary>The tests skipped.</summary>
        private static int _testsSkipped = 0;

        /// <summary>The tests passed.</summary>
        private static int _testsPassed = 0;

        /// <summary>The tests failed.</summary>
        private static int _testsFailed = 0;

        /// <summary>True to fixed format statistics.</summary>
        private static bool _fixedFormatStats = false;

        /// <summary>True to use standard error.</summary>
        private static bool _useStdErr = false;

        /// <summary>The FHIR CodeGen Test CLI.</summary>
        /// <param name="repoRootPath">         The path to the repository root (if not CWD).</param>
        /// <param name="verbose">              True to display all output.</param>
        /// <param name="fixedFormatStatistics">True to output *only* test run statistics:
        ///  #run[tab]#passed[tab]#failed[tab]#skipped.</param>
        /// <param name="errorsToStdError">     True to write errors to stderr instead of stdout.</param>
        /// <returns>Exit-code for the process - returns the number of errors detected (0 for success).</returns>
        public static int Main(
            string repoRootPath = "",
            bool verbose = false,
            bool fixedFormatStatistics = false,
            bool errorsToStdError = false)
        {
            if (string.IsNullOrEmpty(repoRootPath))
            {
                repoRootPath = Directory.GetCurrentDirectory();
            }

            _fixedFormatStats = fixedFormatStatistics;
            _useStdErr = errorsToStdError;

            if (!TryFindRepoRoot(repoRootPath, out string path))
            {
                _testsFailed++;
                WriteTestInfo($"Failed to find repository root: {repoRootPath}!");
                return 1;
            }

            _verbose = verbose;

            try
            {
                _testsRun++;
                //RunCodeGen(path);
                _testsPassed++;
            }
            catch (Exception ex)
            {
                _testsFailed++;
                WriteTestInfo($"Failed during Code Generation! {ex.Message}");

                return 1;
            }

            for (int version = FhirVersionMin; version <= FhirVersionMax; version++)
            {
                try
                {
                    _testsRun++;
                    RunCSharpBasicTest(path, version);
                    _testsPassed++;
                }
                catch (Exception ex)
                {
                    _testsFailed++;
                    WriteTestInfo(ex.Message);
                }
            }

            if (CanCompileTypeScript())
            {
                for (int version = FhirVersionMin; version <= FhirVersionMax; version++)
                {
                    try
                    {
                        _testsRun++;
                        RunTypeScriptTest(path, version);
                        _testsPassed++;
                    }
                    catch (Exception ex)
                    {
                        _testsFailed++;
                        WriteTestInfo(ex.Message);
                    }
                }
            }
            else
            {
                _testsSkipped += (FhirVersionMax - FhirVersionMin);
            }

            WriteTestInfo();

            return _testsFailed;
        }

        /// <summary>Writes the test information.</summary>
        /// <param name="message">(Optional) The message.</param>
        private static void WriteTestInfo(string message = "")
        {
            float passed = (float)_testsPassed / (float)_testsRun;
            float failed = (float)_testsFailed / (float)_testsRun;
            float skipped = (float)_testsSkipped / (float)_testsRun;

            if ((_testsFailed > 0) && (_useStdErr))
            {
                if (_fixedFormatStats)
                {
                    Console.Error.WriteLine($"{_testsRun}\t{_testsPassed}\t{_testsFailed}\t{_testsSkipped}");
                    return;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    Console.Error.WriteLine(message);
                }

                Console.Error.WriteLine("Test Statistics:");
                Console.Error.WriteLine($"Run:     {_testsRun,4}");
                Console.Error.WriteLine($"Passed:  {_testsPassed,4} ({passed:P})");
                Console.Error.WriteLine($"Failed:  {_testsFailed,4} ({failed:P})");
                Console.Error.WriteLine($"Skipped: {_testsSkipped,4} ({skipped:P})");

                return;
            }

            if (_fixedFormatStats)
            {
                Console.WriteLine($"{_testsRun}\t{_testsPassed}\t{_testsFailed}\t{_testsSkipped}");
                return;
            }

            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message);
            }

            Console.WriteLine("Test Statistics:");
            Console.WriteLine($"Run:     {_testsRun,4}");
            Console.WriteLine($"Passed:  {_testsPassed,4} ({passed:P})");
            Console.WriteLine($"Failed:  {_testsFailed,4} ({failed:P})");
            Console.WriteLine($"Skipped: {_testsSkipped,4} ({skipped:P})");
        }

        /// <summary>Determine if we can compile type script.</summary>
        /// <returns>True if we can compile type script, false if not.</returns>
        private static bool CanCompileTypeScript()
        {
            RunAndWait(
                "tsc",
                "--version",
                true,
                out int status);

            if (status == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>Executes the TypeScript test operation.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="repoRoot">The repo root.</param>
        /// <param name="version"> The version.</param>
        private static void RunTypeScriptTest(string repoRoot, int version)
        {
            string project = Path.Combine(repoRoot, $"test/TypeScript_R{version}/tsconfig.json");

            string args =
                $"--diagnostics" +
                $" -p \"{project}\"";

            if (!_fixedFormatStats)
            {
                Console.WriteLine($"Testing TypeScript_R{version}, this may take a few minutes...");
            }

            RunAndWait(
                "tsc",
                args,
                true,
                out int status);

            if (status != 0)
            {
                throw new Exception($"Testing TypeScript_R{version} failed! Returned: {status}");
            }

            if (!_fixedFormatStats)
            {
                Console.WriteLine($"Test of TypeScript_R{version} successful!");
            }
        }

        /// <summary>Executes the C# basic test operation.</summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <param name="repoRoot">The repo root.</param>
        /// <param name="version"> The version.</param>
        private static void RunCSharpBasicTest(string repoRoot, int version)
        {
            string project = Path.Combine(repoRoot, $"test/CSharpBasic_R{version}/CSharpBasic.csproj");

            string args =
                $"build" +
                $" --no-incremental" +
                $" \"{project}\"";

            if (!_fixedFormatStats)
            {
                Console.WriteLine($"Testing CSharpBasic_R{version}, this may take a few minutes...");
            }

            RunAndWait(
                "dotnet",
                args,
                false,
                out int status);

            if (status != 0)
            {
                throw new Exception($"Testing CSharpBasic_R{version} failed! Returned: {status}");
            }

            if (!_fixedFormatStats)
            {
                Console.WriteLine($"Test of CSharpBasic_R{version} successful!");
            }
        }

        /// <summary>Attempts to find repo root a string from the given string.</summary>
        /// <param name="path">Full pathname of the file.</param>
        /// <param name="root">[out] The root.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool TryFindRepoRoot(string path, out string root)
        {
            try
            {
                if (File.Exists(Path.Combine(path, CodegenCsproj)))
                {
                    root = path;
                    return true;
                }
            }
            catch (Exception)
            {
                root = string.Empty;
                return false;
            }

            return TryFindRepoRoot(Path.Combine(path, ".."), out root);
        }

        /// <summary>Executes the code generate operation.</summary>
        /// <param name="repoRoot">The repo root.</param>
        private static void RunCodeGen(string repoRoot)
        {
            string fhirVersions = Path.Combine(repoRoot, "fhirVersions");
            string generated = Path.Combine(repoRoot, "generated");

            string project = Path.Combine(repoRoot, CodegenCsproj);

            string args =
                $"run" +
                $" -p \"{project}\"" +
                $" --fhir-spec-directory \"{fhirVersions}\"" +
                $" --output-path \"{generated}\"" +
                $" --load-r2 latest" +
                $" --load-r3 latest" +
                $" --load-r4 latest" +
                $" --load-r5 latest" +
                $" --language TypeScript|CSharpBasic";

            if (!_fixedFormatStats)
            {
                Console.WriteLine("Running code generation, this may take a few minutes...");
            }

            RunAndWait(
                "dotnet",
                args,
                false,
                out int status);

            if (status != 0)
            {
                throw new Exception($"Code Generation Failed! Returned: {status}");
            }

            if (!_fixedFormatStats)
            {
                Console.WriteLine("Code generation successful!");
            }
        }

        /// <summary>Executes the and wait operation.</summary>
        /// <param name="command">        The command.</param>
        /// <param name="arguments">      The arguments.</param>
        /// <param name="useShellExecute">True to use shell execute.</param>
        /// <param name="retVal">         [out] The return value.</param>
        private static void RunAndWait(
            string command,
            string arguments,
            bool useShellExecute,
            out int retVal)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = command;
            proc.StartInfo.Arguments = arguments;

            if (useShellExecute)
            {
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.RedirectStandardOutput = false;
                proc.StartInfo.RedirectStandardError = false;
            }
            else
            {
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
            }

            proc.Start();

            string stdout = string.Empty;
            string stderr = string.Empty;

            if (!useShellExecute)
            {
                stdout = proc.StandardOutput.ReadToEnd();
                stderr = proc.StandardError.ReadToEnd();
            }

            proc.WaitForExit();

            retVal = proc.ExitCode;

            if (((retVal != 0) || (_verbose)) && (!_fixedFormatStats))
            {
                if (!string.IsNullOrEmpty(stdout))
                {
                    Console.WriteLine("---------stdout----------");
                    Console.WriteLine(stdout);
                    Console.WriteLine("-------------------------");
                }

                if (!string.IsNullOrEmpty(stderr))
                {
                    Console.WriteLine("---------stderr----------");
                    Console.WriteLine(stderr);
                    Console.WriteLine("-------------------------");
                }

            }
        }
    }
}
