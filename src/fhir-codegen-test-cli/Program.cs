// <copyright file="Program.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Diagnostics;
using System.IO;

namespace fhir_codegen_test_cli
{
    /// <summary>The FHIR CodeGen Test CLI</summary>
    public static class Program
    {
        private const string CodegenCsproj = "src/fhir-codegen-cli/fhir-codegen-cli.csproj";

        /// <summary>True to verbose.</summary>
        private static bool _verbose = false;

        /// <summary>The FHIR CodeGen Test CLI.</summary>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <param name="repoRootPath">The path to the repository root (if not CWD).</param>
        /// <param name="verbose">     True to display all output.</param>
        public static int Main(
            string repoRootPath = "",
            bool verbose = false)
        {
            if (string.IsNullOrEmpty(repoRootPath))
            {
                repoRootPath = Directory.GetCurrentDirectory();
            }

            if (!TryFindRepoRoot(repoRootPath, out string path))
            {
                Console.WriteLine($"Failed to find repository root: {repoRootPath}!");
                return -1;
            }

            _verbose = verbose;

            try
            {
                RunCodeGen(path);

                for (int version = 2; version <= 5; version++)
                {
                    RunCSharpBasicTest(path, version);
                    RunTypeScriptTest(path, version);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed! {ex.Message}");
                return -1;
            }

            return 0;
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

            Console.WriteLine($"Testing TypeScript_R{version}, this may take a few minutes...");

            RunAndWait(
                "tsc",
                args,
                true,
                out int status);

            if (status != 0)
            {
                throw new Exception($"Testing TypeScript_R{version} failed! Returned: {status}");
            }

            Console.WriteLine($"Test of TypeScript_R{version} successful!");
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

            Console.WriteLine($"Testing CSharpBasic_R{version}, this may take a few minutes...");

            RunAndWait(
                "dotnet",
                args,
                false,
                out int status);

            if (status != 0)
            {
                throw new Exception($"Testing CSharpBasic_R{version} failed! Returned: {status}");
            }

            Console.WriteLine($"Test of CSharpBasic_R{version} successful!");
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

            Console.WriteLine("Running code generation, this may take a few minutes...");

            RunAndWait(
                "dotnet",
                args,
                false,
                out int status);

            if (status != 0)
            {
                throw new Exception($"Code Generation Failed! Returned: {status}");
            }

            Console.WriteLine("Code generation successful!");
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

            if ((retVal != 0) || (_verbose))
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
