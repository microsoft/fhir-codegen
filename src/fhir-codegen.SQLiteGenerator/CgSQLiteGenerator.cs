using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace fhir_codegen.SQLiteGenerator;

[Generator]
public sealed class CgSQLiteGenerator : IIncrementalGenerator
{
    //private const string _joiner_0 = ",\r\n";
    //private const string _joiner_1 = ",\r\n    ";
    //private const string _joiner_2 = ",\r\n        ";
    private const string _line_2 = "\r\n        ";
    private const string _line_3 = "\r\n            ";
    private const string _line_4 = "\r\n                ";
    private const string _comma_line_2 = ",\r\n        ";
    private const string _comma_line_4 = ",\r\n                ";
    private const string _comma_line_5 = ",\r\n                    ";

    private enum CgGenCategory
    {
        Class,
        Record,
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DO_NOT_ATTACH_DEBUGGER
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Launch();
        }
#endif

        // create a generated file with our attributes so the target project can use them
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CgSQLiteGeneratorAttributes.g.cs",
            SourceText.From(GeneratorAttributes.CgAttributes, Encoding.UTF8)));

        IncrementalValuesProvider<ClassDeclarationSyntax> cDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetClassDec(s),
                transform: static (ctx, _) => GetClassTargetForGeneration(ctx));

        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> ivpClasses =
                context.CompilationProvider.Combine(cDeclarations.Collect());

        context.RegisterSourceOutput(ivpClasses, (spc, source) => Execute(source.Item1, source.Item2, spc));


        IncrementalValuesProvider<RecordDeclarationSyntax> rDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetRecordDec(s),
                transform: static (ctx, _) => GetRecordTargetForGeneration(ctx));

        IncrementalValueProvider<(Compilation, ImmutableArray<RecordDeclarationSyntax>)> ivpRecords =
                context.CompilationProvider.Combine(rDeclarations.Collect());

        context.RegisterSourceOutput(ivpRecords, (spc, source) => Execute(source.Item1, source.Item2, spc));
    }


    /// <summary>
    /// Determines if the given <see cref="SyntaxNode"/> is a target for generation.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to evaluate.</param>
    /// <returns><c>true</c> if the syntax node is a class declaration with specific attributes; otherwise, <c>false</c>.</returns>
    public static bool IsSyntaxTargetClassDec(SyntaxNode syntaxNode)
    {
        return
            (syntaxNode is ClassDeclarationSyntax cDeclarationSyntax) &&
            (cDeclarationSyntax.AttributeLists.Count > 0) &&
            cDeclarationSyntax.AttributeLists.Any(al => al.Attributes.Any(a => GeneratorAttributes._cgClassAttributes.Contains(a.Name.ToString())));
    }


    /// <summary>
    /// Determines if the given <see cref="SyntaxNode"/> is a target for generation.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to evaluate.</param>
    /// <returns><c>true</c> if the syntax node is a class declaration with specific attributes; otherwise, <c>false</c>.</returns>
    public static bool IsSyntaxTargetRecordDec(SyntaxNode syntaxNode)
    {
        return
            (syntaxNode is RecordDeclarationSyntax rDeclarationSyntax) &&
            (rDeclarationSyntax.AttributeLists.Count > 0) &&
            rDeclarationSyntax.AttributeLists.Any(al => al.Attributes.Any(a => GeneratorAttributes._cgClassAttributes.Contains(a.Name.ToString())));
    }


    /// <summary>
    /// Retrieves the target class declaration syntax for generation.
    /// </summary>
    /// <param name="context">The generator syntax context.</param>
    /// <returns>The class declaration syntax node.</returns>
    public static ClassDeclarationSyntax GetClassTargetForGeneration(GeneratorSyntaxContext context)
    {
        ClassDeclarationSyntax cDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        return cDeclarationSyntax;
    }

    /// <summary>
    /// Retrieves the target class declaration syntax for generation.
    /// </summary>
    /// <param name="context">The generator syntax context.</param>
    /// <returns>The record declaration syntax node.</returns>
    public static RecordDeclarationSyntax GetRecordTargetForGeneration(GeneratorSyntaxContext context)
    {
        RecordDeclarationSyntax rDeclarationSyntax = (RecordDeclarationSyntax)context.Node;
        return rDeclarationSyntax;
    }

    public void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        ILookup<string, ClassDeclarationSyntax> classLookup = classes.ToLookup(c => c.Identifier.Text);

        foreach (ClassDeclarationSyntax classSyntax in classes.Where(c => c.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == GeneratorAttributes._cgSQLiteTable))))
        {
            // Converting the class to a semantic model to access much more meaningful data.
            SemanticModel model = compilation.GetSemanticModel(classSyntax.SyntaxTree);

            // Parse to declared symbol, so you can access each part of code separately,
            // such as interfaces, methods, members, contructor parameters etc.
            ISymbol? symbol = model.GetDeclaredSymbol(classSyntax);

            if (symbol == null)
            {
                continue;
            }

            List<MemberDeclarationSyntax> members = [];

            string? btn = ((INamedTypeSymbol)symbol).BaseType?.Name;
            while (btn != null)
            {
                ClassDeclarationSyntax? btcs = classLookup[btn].FirstOrDefault();
                if (btcs == null)
                {
                    break;
                }

                members.AddRange(btcs.Members);
                btn = compilation.GetSemanticModel(btcs.SyntaxTree).GetDeclaredSymbol(btcs) is INamedTypeSymbol ints
                    ? ints.BaseType?.Name
                    : null;
            }

            members.AddRange(classSyntax.Members);

            execute(
                compilation,
                symbol,
                members,
                context,
                CgGenCategory.Class);
        }
    }

    public void Execute(
        Compilation compilation,
        ImmutableArray<RecordDeclarationSyntax> records,
        SourceProductionContext context)
    {
        ILookup<string, RecordDeclarationSyntax> recordLookup = records.ToLookup(c => c.Identifier.Text);

        foreach (RecordDeclarationSyntax recordSyntax in records.Where(c => c.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == GeneratorAttributes._cgSQLiteTable))))
        {
            // Converting the record to a semantic model to access much more meaningful data.
            SemanticModel model = compilation.GetSemanticModel(recordSyntax.SyntaxTree);

            // Parse to declared symbol, so you can access each part of code separately,
            // such as interfaces, methods, members, contructor parameters etc.
            ISymbol? symbol = model.GetDeclaredSymbol(recordSyntax);

            if (symbol == null)
            {
                continue;
            }

            List<MemberDeclarationSyntax> members = [];
            string? btn = ((INamedTypeSymbol)symbol).BaseType?.Name;
            while (btn != null)
            {
                RecordDeclarationSyntax? btcs = recordLookup[btn].FirstOrDefault();
                if (btcs == null)
                {
                    break;
                }

                members.AddRange(btcs.Members);
                btn = compilation.GetSemanticModel(btcs.SyntaxTree).GetDeclaredSymbol(btcs) is INamedTypeSymbol ints
                    ? ints.BaseType?.Name
                    : null;
            }

            members.AddRange(recordSyntax.Members);

            execute(
                compilation,
                symbol,
                members,
                context,
                CgGenCategory.Record);
        }
    }


    private void execute(
        Compilation compilation,
        ISymbol symbol,
        List<MemberDeclarationSyntax> members,
        SourceProductionContext context,
        CgGenCategory genCategory)
    {
        string className = symbol.Name;
        string? classNamespace = symbol.ContainingNamespace?.ToDisplayString();
        string? classAssembly = symbol.ContainingAssembly?.Name;

        ILookup<string?, AttributeData> symbolAttributeLookup = symbol.GetAttributes().ToLookup(a => a.AttributeClass?.Name);

        string tableName = symbolAttributeLookup.Contains(GeneratorAttributes._cgSQLiteTable)
            ? symbolAttributeLookup[GeneratorAttributes._cgSQLiteTable].First().ConstructorArguments.FirstOrDefault().Value?.ToString() ?? className
            : className;

        int? pkColIndex = null;
        string? pkColName = null;
        string? pkPropType = null;
        bool pkIsIdentity = false;

        List<string> createColLines = [];
        List<string> createFKLines = [];
        List<(string name, string propType, string shortRead, string readerDirective, bool isPrimaryKey, bool isIdentity, bool isNullable, bool isEnum)> tableColInfo = [];

        foreach (MemberDeclarationSyntax member in members)
        {
            // only process properties
            if (member is not PropertyDeclarationSyntax pds)
            {
                continue;
            }

            SemanticModel pModel = compilation.GetSemanticModel(pds.SyntaxTree);
            //ISymbol? pSymbol = pModel.GetDeclaredSymbol(pds);

            //if (pSymbol == null)
            //{
            //    continue;
            //}

            // check for ignore property
            if (pds.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == GeneratorAttributes._cgSQLiteIgnore)))
            {
                continue;
            }

            string propName = pds.Identifier.ToString();

            TypeInfo propTypeInfo = pModel.GetTypeInfo(pds.Type);
            string propTypeName = pds.Type.ToString();

            bool nullable = propTypeName.EndsWith("?");
            if (nullable)
            {
                propTypeName = propTypeName.Substring(0, propTypeName.Length - 1);
            }

            // check for primary key property
            bool isPrimaryKey = pds.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == GeneratorAttributes._cgSQLiteKey));
            if (isPrimaryKey)
            {
                pkColName = propName;
                pkPropType = propTypeName;
                pkIsIdentity = (propTypeName == "int" || propTypeName == "long");
            }

            if (isPrimaryKey)
            {
                if (pkColName == null)
                {
                    throw new Exception("Primary key column name is null");
                }

                if (pkColName != propName)
                {
                    throw new Exception("Primary key column name does not match property name");
                }

                pkColIndex = tableColInfo.Count;
            }

            // check for type nullability
            //bool nullable = Nullable.GetUnderlyingType(member.) != null;
            //bool nullable = new NullabilityInfoContext().Create(prop).WriteState is NullabilityState.Nullable;

            string? enumTypeName = null;
            bool memberIsEnum = false;

            if ((propTypeInfo.Type is INamedTypeSymbol ints) &&
                (
                    (ints.TypeKind == TypeKind.Enum) ||
                    ((ints.TypeArguments.Length != 0) && (ints.TypeArguments[0].TypeKind == TypeKind.Enum)) ||
                    (ints.TypeKind == TypeKind.Struct) && (ints.TypeArguments.Length != 0) && ints.TypeArguments[0].ContainingNamespace.Name.StartsWith("HL7")
                ))
            {
                memberIsEnum = true;

                // grab the enum type name
                if (ints.TypeKind == TypeKind.Enum)
                {
                    if (ints.ContainingType == null)
                    {
                        enumTypeName = $"{ints.ContainingNamespace}.{ints.Name}";
                    }
                    else
                    {
                        enumTypeName = $"{ints.ContainingType.ContainingNamespace}.{ints.ContainingType.Name}.{ints.Name}";
                    }
                }
                else if (ints.TypeArguments.Length != 0)
                {
                    if (ints.TypeArguments[0].ContainingType == null)
                    {
                        enumTypeName = $"{ints.TypeArguments[0].ContainingNamespace}.{ints.TypeArguments[0].Name}";
                    }
                    else
                    {
                        enumTypeName = $"{ints.TypeArguments[0].ContainingType.ContainingNamespace}.{ints.TypeArguments[0].ContainingType.Name}.{ints.TypeArguments[0].Name}";
                    }
                }
                else
                {
                    enumTypeName = ints.Name;
                }
            }

            // add our column line
            createColLines.Add(
                $"{pds.Identifier.ToString()} {getSqlType(propTypeName, memberIsEnum)}" +
                $"{(isPrimaryKey ? getPkDirective(pkPropType) : string.Empty)}" +
                $"{((nullable || isPrimaryKey) ? string.Empty : " NOT NULL")}");

            // check for foreign key property information
            string? foreignTable = null;
            string? foreignColumn = null;
            foreach (AttributeListSyntax als in pds.AttributeLists)
            {
                foreach (AttributeSyntax a in als.Attributes.Where(a => a.Name.ToString() == GeneratorAttributes._cgSQLiteForeignKey))
                {
                    foreach (AttributeArgumentSyntax arg in a.ArgumentList?.Arguments ?? [])
                    {
                        if (arg.NameEquals?.Name.ToString() == "ReferenceTable")
                        {
                            foreignTable = arg.Expression.ToString();
                        }
                        else if (arg.NameEquals?.Name.ToString() == "ReferenceColumn")
                        {
                            foreignColumn = arg.Expression.ToString();
                        }
                    }
                }
            }

            if ((foreignTable != null) && (foreignColumn != null))
            {
                createFKLines.Add($"FOREIGN KEY ({pds.Identifier}) REFERENCES {foreignTable}({foreignColumn})");
            }

            // create the select retrieval pair
            if (nullable && _sqliteNullableReadDirectives.TryGetValue(propTypeName, out string? readFormat))
            {
                tableColInfo.Add((
                    propName,
                    propTypeName,
                    string.Format(readFormat.Remove(0, 6), pds.Identifier.ToString(), "reader", tableColInfo.Count),
                    string.Format(readFormat, pds.Identifier.ToString(), "reader", tableColInfo.Count),
                    isPrimaryKey,
                    isPrimaryKey && (propTypeName == "int" || propTypeName == "long"),
                    nullable,
                    memberIsEnum
                    ));
            }
            else if (!nullable && _sqliteReadDirectives.TryGetValue(propTypeName, out readFormat))
            {
                tableColInfo.Add((
                    propName,
                    propTypeName,
                    string.Format(readFormat.Remove(0, 6), pds.Identifier.ToString(), "reader", tableColInfo.Count),
                    string.Format(readFormat, pds.Identifier.ToString(), "reader", tableColInfo.Count),
                    isPrimaryKey,
                    isPrimaryKey && (propTypeName == "int" || propTypeName == "long"),
                    nullable,
                    memberIsEnum
                    ));
            }
            else if (memberIsEnum)
            {
                //// build the reader directive for the enum type
                //string ef = $"Enum.TryParse(reader.GetString({tableColInfo.Count}), out {propName});";

                tableColInfo.Add((
                    propName,
                    propTypeName,
                    nullable
                        ? string.Format(_sqliteNullableReadDirectives["enum"].Remove(0, 6), pds.Identifier.ToString(), "reader", tableColInfo.Count, enumTypeName)
                        : string.Format(_sqliteReadDirectives["enum"].Remove(0, 6), pds.Identifier.ToString(), "reader", tableColInfo.Count, enumTypeName),
                    nullable
                        ? string.Format(_sqliteNullableReadDirectives["enum"], pds.Identifier.ToString(), "reader", tableColInfo.Count, enumTypeName)
                        : string.Format(_sqliteReadDirectives["enum"], pds.Identifier.ToString(), "reader", tableColInfo.Count, enumTypeName),
                    isPrimaryKey,
                    isPrimaryKey && (propTypeName == "int" || propTypeName == "long"),
                    nullable,
                    memberIsEnum
                    ));
            }
            else
            {
                tableColInfo.Add((
                    pds.Identifier.ToString(),
                    propTypeName,
                    $"// ERROR: could not determine retrieval directive for type {propName}:{propTypeName}",
                    $"// ERROR: could not determine retrieval directive for type {propName}:{propTypeName}",
                    isPrimaryKey,
                    isPrimaryKey && (propTypeName == "int" || propTypeName == "long"),
                    nullable,
                    memberIsEnum
                    ));
            }
        }

        context.AddSource(
            $"{className}{"SQLite"}.g.cs",
            SourceText.From($$$""""
                    //------------------------------------------------------------------------------
                    // <auto-generated>
                    //     This code was generated by a tool.
                    //
                    //     Changes to this file may cause incorrect behavior and will be lost if
                    //     the code is regenerated.
                    // </auto-generated>
                    //------------------------------------------------------------------------------

                    #nullable enable

                    using System.Collections.Generic;
                    using System.Data;
                    using System.Text;
                
                    namespace {{{classNamespace}}};
                
                    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
                    public partial {{{decForGenCategory(genCategory)}}} {{{className}}}
                    {
                        public static string DefaultTableName => "{{{tableName}}}";

                        public static bool CreateTable(IDbConnection dbConnection, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";

                            IDbCommand command = dbConnection.CreateCommand();
                            command.CommandText = $"""
                                CREATE TABLE IF NOT EXISTS {dbTableName} (
                                    {{{string.Join(_comma_line_4, [.. createColLines, .. createFKLines])}}}
                                )
                                """;

                            command.ExecuteNonQuery();

                            {{{string.Join(_line_2, getIndexLines())}}}
                    
                            return true;
                        }

                        public static bool DropTable(IDbConnection dbConnection, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                    
                            IDbCommand command = dbConnection.CreateCommand();
                            command.CommandText = $"DROP TABLE IF EXISTS {dbTableName}";
                    
                            command.ExecuteNonQuery();
                    
                            return true;
                        }


                        public static {{{(pkColName == null ? "int" : pkPropType)}}}? SelectMaxKey(IDbConnection dbConnection, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                    
                            IDbCommand command = dbConnection.CreateCommand();
                            command.CommandText = $"SELECT MAX({{{(pkColName == null ? "ROWID" : pkColName)}}}) FROM {dbTableName}";

                            object? result = command.ExecuteScalar();
                            if (result is {{{(pkColName == null ? "int" : pkPropType)}}} value)
                            {
                                return value;
                            }
                            else if (result is long l)
                            {
                                return {{{((pkColName == null) || (pkPropType == "int") ? "Convert.ToInt32(l)" : "null")}}};
                            }

                            return null;
                        }
                    
                        public static {{{className}}}? SelectSingle(IDbConnection dbConnection, string? dbTableName = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                        {
                            dbTableName ??= "{{{tableName}}}";

                            IDbCommand command = dbConnection.CreateCommand();
                            command.CommandText = $"SELECT {{{string.Join(", ", tableColInfo.Select(p => p.name))}}} FROM {dbTableName}";

                            bool addedCondition = false;

                            {{{string.Join(_line_2, getConditionLines(true))}}}

                            using (IDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    return new()
                                    {
                                        {{{string.Join(_comma_line_5, tableColInfo.Select(p => p.readerDirective))}}}
                                    };
                                }
                            }
                            return null;
                        }

                        public static List<{{{className}}}> SelectList(IDbConnection dbConnection, string? dbTableName = null, string[]? orderByProperties = null, string? orderByDirection = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                        {
                            dbTableName ??= "{{{tableName}}}";

                            List<{{{className}}}> results = new();

                            IDbCommand command = dbConnection.CreateCommand();
                            command.CommandText = $"SELECT {{{string.Join(", ", tableColInfo.Select(p => p.name))}}} FROM {dbTableName}";
                    
                            bool addedCondition = false;
                    
                            {{{string.Join(_line_2, getConditionLines(true))}}}

                            if ((orderByProperties != null) && (orderByProperties.Length > 0))
                            {
                                command.CommandText += $" ORDER BY {string.Join(", ", orderByProperties)}";
                                if (orderByDirection?.StartsWith("d", StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    command.CommandText += $" DESC";
                                }
                                else
                                {
                                    command.CommandText += $" ASC";
                                }
                            }
                    
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    results.Add(new()
                                    {
                                        {{{string.Join(_comma_line_5, tableColInfo.Select(p => p.readerDirective))}}}
                                    });
                                }
                            }
                            return results;
                        }

                        public static Dictionary<{{{(pkColName == null ? "int" : pkPropType)}}}, {{{className}}}> SelectDict(IDbConnection dbConnection, string? dbTableName = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                        {
                            dbTableName ??= "{{{tableName}}}";
                    
                            Dictionary<{{{(pkColName == null ? "int" : pkPropType)}}}, {{{className}}}> results = new();
                    
                            IDbCommand command = dbConnection.CreateCommand();
                            command.CommandText = $"SELECT {{{string.Join(", ", tableColInfo.Select(p => p.name))}}} FROM {dbTableName}";
                    
                            bool addedCondition = false;
                    
                            {{{string.Join(_line_2, getConditionLines(true))}}}

                            {{{(pkColName == null ? "int rowId = 0;" : string.Empty)}}}
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    results.Add({{{(pkColName == null ? "rowId++" : tableColInfo[(int)pkColIndex!].shortRead)}}}, new()
                                    {
                                        {{{string.Join(_comma_line_5, tableColInfo.Select(p => p.readerDirective))}}}
                                    });
                                }
                            }
                            return results;
                        }

                        public static int SelectCount(IDbConnection dbConnection, string? dbTableName = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                        {
                            dbTableName ??= "{{{tableName}}}";
                    
                            IDbCommand command = dbConnection.CreateCommand();
                            command.CommandText = $"SELECT COUNT({{{(pkColName == null ? "*" : pkColName)}}}) FROM {dbTableName}";
                    
                            bool addedCondition = false;
                    
                            {{{string.Join(_line_2, getConditionLines(true))}}}

                            object? result = command.ExecuteScalar();
                            if (result is int value)
                            {
                                return value;
                            }
                            else if (result is long l)
                            {
                                return {{{((pkColName == null) || (pkPropType == "int") ? "Convert.ToInt32(l)" : "null")}}};
                            }

                            return -1;
                        }

                        public static {{{(pkColName == null ? "void" : pkPropType)}}} Insert(IDbConnection dbConnection, {{{className}}} value, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                            {{{getNonIdentityPkInit(pkColName, pkPropType)}}}
                            using (IDbTransaction transaction = dbConnection.BeginTransaction())
                            {
                                IDbCommand command = dbConnection.CreateCommand();
                                command.CommandText = $"""
                                    INSERT INTO {dbTableName} (
                                        {{{string.Join(_comma_line_5, tableColInfo.Where(p => p.isIdentity == false).Select(p => p.name))}}}
                                    ) VALUES (
                                        {{{string.Join(_comma_line_5, tableColInfo.Where(p => p.isIdentity == false).Select(p => "$" + p.name))}}}
                                    ) {{{(pkIsIdentity ? " RETURNING " + pkColName : string.Empty)}}};
                                    """;

                                {{{string.Join(_line_3, getInsertCommandParamLines(true, pkIsIdentity ? pkColName : null, pkPropType))}}}

                                transaction.Commit();
                            }

                            {{{(pkColName == null ? "return" : $"return value.{pkColName}")}}};
                        }

                        public static void Insert(IDbConnection dbConnection, List<{{{className}}}> values, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                            
                            using (IDbTransaction transaction = dbConnection.BeginTransaction())
                            {
                                IDbCommand command = dbConnection.CreateCommand();
                                command.CommandText = $"""
                                    INSERT INTO {dbTableName} (
                                        {{{string.Join(_comma_line_5, tableColInfo.Where(p => p.isIdentity == false).Select(p => p.name))}}}
                                    ) VALUES (
                                        {{{string.Join(_comma_line_5, tableColInfo.Where(p => p.isIdentity == false).Select(p => "$" + p.name))}}}
                                    ) {{{(pkIsIdentity ? " RETURNING " + pkColName : string.Empty)}}};
                                    """;

                                {{{string.Join(_line_3, getInsertCommandParamLines(false, pkIsIdentity ? pkColName : null, pkPropType, createParameters: true))}}}

                                command.Prepare();

                                foreach ({{{className}}} value in values)
                                {
                                    {{{getNonIdentityPkInit(pkColName, pkPropType)}}}
                                    {{{string.Join(_line_4, getInsertCommandParamLines(false, pkIsIdentity ? pkColName : null, pkPropType, instantiateParameters: true, executeCommand: true))}}}
                                }
                    
                                transaction.Commit();
                            }
                        }

                        public static void Insert(IDbConnection dbConnection, IEnumerable<{{{className}}}> values, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                            
                            using (IDbTransaction transaction = dbConnection.BeginTransaction())
                            {
                                IDbCommand command = dbConnection.CreateCommand();
                                command.CommandText = $"""
                                    INSERT INTO {dbTableName} (
                                        {{{string.Join(_comma_line_5, tableColInfo.Where(p => p.isIdentity == false).Select(p => p.name))}}}
                                    ) VALUES (
                                        {{{string.Join(_comma_line_5, tableColInfo.Where(p => p.isIdentity == false).Select(p => "$" + p.name))}}}
                                    ) {{{(pkIsIdentity ? " RETURNING " + pkColName : string.Empty)}}};
                                    """;
                    
                                {{{string.Join(_line_3, getInsertCommandParamLines(false, pkIsIdentity ? pkColName : null, pkPropType, createParameters: true))}}}
                    
                                command.Prepare();
                    
                                foreach ({{{className}}} value in values)
                                {
                                    {{{getNonIdentityPkInit(pkColName, pkPropType)}}}
                                    {{{string.Join(_line_4, getInsertCommandParamLines(false, pkIsIdentity ? pkColName : null, pkPropType, instantiateParameters: true, executeCommand: true, setIdentity: false))}}}
                                }
                    
                                transaction.Commit();
                            }
                        }

                        public static {{{className}}} Update(IDbConnection dbConnection, {{{className}}} value, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                    
                            using (IDbTransaction transaction = dbConnection.BeginTransaction())
                            {
                                IDbCommand command = dbConnection.CreateCommand();
                                command.CommandText = $"""
                                    UPDATE {dbTableName} SET
                                        {{{string.Join(_comma_line_5, tableColInfo.Where(p => p.isPrimaryKey == false).Select(p => p.name + " = $" + p.name))}}}
                                    WHERE
                                        {{{pkColName}}} = ${{{pkColName}}}
                                    """;
                    
                                {{{string.Join(_line_3, getInsertCommandParamLines(true, pkIsIdentity ? pkColName : null, pkPropType, includeIdentity: true, isInsert: false))}}}
                    
                                transaction.Commit();
                            }
                    
                            return value;
                        }

                        public static void Update(IDbConnection dbConnection, IEnumerable<{{{className}}}> values, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                            
                            using (IDbTransaction transaction = dbConnection.BeginTransaction())
                            {
                                IDbCommand command = dbConnection.CreateCommand();
                                command.CommandText = $"""
                                    UPDATE {dbTableName} SET
                                        {{{string.Join(_comma_line_5, tableColInfo.Where(p => p.isPrimaryKey == false).Select(p => p.name + " = $" + p.name))}}}
                                    WHERE
                                        {{{pkColName}}} = ${{{pkColName}}}
                                    """;
                    
                                {{{string.Join(
                                _line_3,
                                getInsertCommandParamLines(
                                    false,
                                    pkIsIdentity ? pkColName : null,
                                    pkPropType,
                                    createParameters: true,
                                    includeIdentity: true,
                                    isInsert: false))}}}
                    
                                foreach ({{{className}}} value in values)
                                {
                                    {{{string.Join(
                                    _line_4,
                                    getInsertCommandParamLines(
                                        false,
                                        pkIsIdentity ? pkColName : null,
                                        pkPropType,
                                        instantiateParameters: true,
                                        executeCommand: true,
                                        setIdentity: false,
                                        includeIdentity: true,
                                        isInsert: false))}}}
                                }
                    
                                transaction.Commit();
                            }
                        }

                        public static void Delete(IDbConnection dbConnection, {{{className}}} value, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                    
                            using (IDbTransaction transaction = dbConnection.BeginTransaction())
                            {
                                IDbCommand command = dbConnection.CreateCommand();
                                command.CommandText = $"DELETE FROM {dbTableName} WHERE {{{pkColName}}} = ${{{pkColName}}}";
                    
                                {{{string.Join(
                                _line_3,
                                getInsertCommandParamLines(
                                    true,
                                    pkIsIdentity ? pkColName : null,
                                    pkPropType,
                                    includeIdentity: true,
                                    identityOnly: true,
                                    setIdentity: false))}}}
                    
                                transaction.Commit();
                            }
                        }
                    
                        public static void Delete(IDbConnection dbConnection, IEnumerable<{{{className}}}> values, string? dbTableName = null)
                        {
                            dbTableName ??= "{{{tableName}}}";
                            
                            using (IDbTransaction transaction = dbConnection.BeginTransaction())
                            {
                                IDbCommand command = dbConnection.CreateCommand();
                                command.CommandText = $"DELETE FROM {dbTableName} WHERE {{{pkColName}}} = ${{{pkColName}}}";
                                        
                                {{{string.Join(
                                _line_3,
                                getInsertCommandParamLines(
                                    false,
                                    pkIsIdentity ? pkColName : null,
                                    pkPropType,
                                    createParameters: true,
                                    includeIdentity: true,
                                    identityOnly: true,
                                    setIdentity: false))}}}
                    
                                foreach ({{{className}}} value in values)
                                {
                                    {{{string.Join(
                                    _line_4,
                                    getInsertCommandParamLines(
                                        false,
                                        pkIsIdentity ? pkColName : null,
                                        pkPropType,
                                        instantiateParameters: true,
                                        executeCommand: true,
                                        setIdentity: false,
                                        includeIdentity: true,
                                        identityOnly: true))}}}
                                }
                    
                                transaction.Commit();
                            }
                        }

                        public static void Delete(IDbConnection dbConnection, string? dbTableName = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                        {
                            dbTableName ??= "{{{tableName}}}";
                    
                            using (IDbTransaction transaction = dbConnection.BeginTransaction())
                            {
                                IDbCommand command = dbConnection.CreateCommand();
                                command.CommandText = $"DELETE FROM {dbTableName}";
                                        
                                bool addedCondition = false;
                    
                                {{{string.Join(_line_2, getConditionLines(true))}}}

                                transaction.Commit();
                            }
                        }
                    }

                    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
                    public static class {{{className}}}Extensions
                    {
                        public static {{{className}}}? SelectSingle<T>(this IDbConnection dbCon, string? dbTableName = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                            where T : {{{className}}}
                        {
                            return {{{className}}}.SelectSingle(dbCon, dbTableName, {{{string.Join(", ", getFnFilterArgs(true))}}});
                        }

                        public static List<{{{className}}}> SelectList<T>(this IDbConnection dbCon, string? dbTableName = null, string[]? orderByProperties = null, string? orderByDirection = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                            where T : {{{className}}}
                        {
                            return {{{className}}}.SelectList(dbCon, dbTableName, orderByProperties, orderByDirection, {{{string.Join(", ", getFnFilterArgs(true))}}});
                        }

                        public static int SelectCount<T>(this IDbConnection dbCon, string? dbTableName = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                            where T : {{{className}}}
                        {
                            return {{{className}}}.SelectCount(dbCon, dbTableName, {{{string.Join(", ", getFnFilterArgs(true))}}});
                        }
                                                            
                        public static void Insert(this IDbConnection dbCon, {{{className}}} value, string? dbTableName = null)
                        {
                            {{{className}}}.Insert(dbCon, value, dbTableName);
                        }

                        public static void Insert(this IDbConnection dbCon, List<{{{className}}}> values, string? dbTableName = null)
                        {
                            {{{className}}}.Insert(dbCon, values, dbTableName);
                        }

                        public static void Insert(this IDbConnection dbCon, IEnumerable<{{{className}}}> values, string? dbTableName = null)
                        {
                            {{{className}}}.Insert(dbCon, values, dbTableName);
                        }

                        public static void Update(this IDbConnection dbCon, {{{className}}} value, string? dbTableName = null)
                        {
                            {{{className}}}.Update(dbCon, value, dbTableName);
                        }

                        public static void Update(this IDbConnection dbCon, IEnumerable<{{{className}}}> values, string? dbTableName = null)
                        {
                            {{{className}}}.Update(dbCon, values, dbTableName);
                        }

                        public static void Delete(this IDbConnection dbCon, {{{className}}} value, string? dbTableName = null)
                        {
                            {{{className}}}.Delete(dbCon, value, dbTableName);
                        }
                    
                        public static void Delete(this IDbConnection dbCon, IEnumerable<{{{className}}}> values, string? dbTableName = null)
                        {
                            {{{className}}}.Delete(dbCon, values, dbTableName);
                        }

                        public static void Delete(this IDbConnection dbCon, string? dbTableName = null, {{{string.Join(", ", getFnFilterParams(true))}}})
                        {
                            {{{className}}}.Delete(dbCon, dbTableName, {{{string.Join(", ", getFnFilterArgs(true))}}});
                        }

                        public static void Insert(this {{{className}}} value, IDbConnection dbCon, string? dbTableName = null)
                        {
                            {{{className}}}.Insert(dbCon, value, dbTableName);
                        }
                    
                        public static void Insert(this List<{{{className}}}> values, IDbConnection dbCon, string? dbTableName = null)
                        {
                            {{{className}}}.Insert(dbCon, values, dbTableName);
                        }

                        public static void Insert(this IEnumerable<{{{className}}}> values, IDbConnection dbCon, string? dbTableName = null)
                        {
                            {{{className}}}.Insert(dbCon, values, dbTableName);
                        }
                    
                        public static void Update(this {{{className}}} value, IDbConnection dbCon, string? dbTableName = null)
                        {
                            {{{className}}}.Update(dbCon, value, dbTableName);
                        }
                    
                        public static void Update(this IEnumerable<{{{className}}}> values, IDbConnection dbCon, string? dbTableName = null)
                        {
                            {{{className}}}.Update(dbCon, values, dbTableName);
                        }

                        public static void Delete(this {{{className}}} value, IDbConnection dbCon, string? dbTableName = null)
                        {
                            {{{className}}}.Delete(dbCon, value, dbTableName);
                        }
                    
                        public static void Delete(this IEnumerable<{{{className}}}> values, IDbConnection dbCon, string? dbTableName = null)
                        {
                            {{{className}}}.Delete(dbCon, values, dbTableName);
                        }
                    }

                    #nullable restore
                    """"
            , Encoding.UTF8)
        );

        return;

        string decForGenCategory(CgGenCategory genCategory) => genCategory switch
        {
            CgGenCategory.Class => "class",
            CgGenCategory.Record => "record class",
            _ => "class",
        };

        string getPkDirective(string? pkTypeName) => pkTypeName switch
        {
            // note that in SQLite, an INTEGER PRIMARY KEY uses the internal ROWID automatically, so no IDENTITY declaration is necessary
            "int" => " PRIMARY KEY",            // " IDENTITY(1,1) PRIMARY KEY",
            "long" => " PRIMARY KEY",           // " IDENTITY(1,1) PRIMARY KEY",
            _ => " PRIMARY KEY",
        };

        string getNonIdentityPkInit(string? pkColName, string? pkTypeName) => pkTypeName switch
        {
            "guid" => $"value.{pkColName} = Guid.NewGuid();",
            _ => string.Empty,
        };

        IEnumerable<string> getIndexLines()
        {
            if (!symbolAttributeLookup.Contains(GeneratorAttributes._cgSQLiteIndex))
            {
                yield break;
            }

            // generate any indexes
            foreach (AttributeData ad in symbolAttributeLookup[GeneratorAttributes._cgSQLiteIndex])
            {
                string[] columns = ad.ConstructorArguments
                    .FirstOrDefault()
                    .Values
                    .Select(tc => tc.Value?.ToString() ?? string.Empty)
                    .Where(v => !string.IsNullOrEmpty(v)).
                    ToArray();

                if (columns.Length == 0)
                {
                    continue;
                }

                string indexName = $"IDX_{{dbTableName}}_{(string.Join("_", columns))}";

                yield return "command = dbConnection.CreateCommand();";
                yield return "command.CommandText = $\"\"\"";
                yield return $"    CREATE INDEX IF NOT EXISTS \"{indexName}\" ON \"{{dbTableName}}\" (";
                yield return $"        {string.Join(", ", columns.Select(v => $"\"{v}\""))}";
                yield return "    )";
                yield return "    \"\"\";";
                yield return "command.ExecuteNonQuery();";
                yield return string.Empty;
            }
        }

        IEnumerable<string> getFnFilterParams(bool includeNullFilter)
        {
            foreach ((string name, string propType, string _, string _, bool _, bool _, bool isNullable, bool isEnum) in tableColInfo)
            {
                yield return $"{propType}? {name} = null";

                if (isNullable)
                {
                    yield return $"bool {name}IsNull = false";
                }
            }
        }

        IEnumerable<string> getFnFilterArgs(bool includeNullFilter)
        {
            foreach ((string name, string propType, string _, string _, bool _, bool _, bool isNullable, bool isEnum) in tableColInfo)
            {
                yield return name;

                if (isNullable)
                {
                    yield return $"{name}IsNull";
                }
            }
        }

        IEnumerable<string> getConditionLines(bool includeNullFilter)
        {
            foreach ((string name, string propType, string _, string _, bool _, bool _, bool isNullable, bool isEnum) in tableColInfo)
            {
                yield return $"if ({name} != null)";
                yield return "{";
                yield return $"    command.CommandText += (addedCondition ? \" AND \" : \" WHERE \") + \"{name} = ${name}\";";
                yield return "    addedCondition = true;";
                yield return string.Empty;
                yield return $"    IDbDataParameter {name}Param = command.CreateParameter();";
                yield return $"    {name}Param.ParameterName = \"${name}\";";

                if (isEnum)
                {
                    yield return $"    {name}Param.Value = {name}.ToString();";
                }
                else
                {
                    yield return $"    {name}Param.Value = {name};";
                }

                yield return $"    command.Parameters.Add({name}Param);";
                yield return "}";
                yield return string.Empty;

                if (includeNullFilter && isNullable)
                {
                    yield return $"if ({name}IsNull)";
                    yield return "{";
                    yield return $"    command.CommandText += (addedCondition ? \" AND \" : \" WHERE \") + \"{name} IS NULL\";";
                    yield return "    addedCondition = true;";
                    yield return "}";
                    yield return string.Empty;
                }
            }
        }

        IEnumerable<string> getInsertCommandParamLines(
            bool singleValue,
            string? identityColName,
            string? identityColType,
            bool? createParameters = null,
            bool? instantiateParameters = null,
            bool? executeCommand = null,
            bool setIdentity = true,
            bool includeIdentity = false,
            bool identityOnly = false,
            bool isInsert = true)
        {
            // if no specific type is specified, default to true
            if ((createParameters == null) && (instantiateParameters == null) && (executeCommand == null))
            {
                createParameters = true;
                instantiateParameters = true;
                executeCommand = true;
            }

            foreach ((string name, string _, string _, string _, bool isPrimaryKey, bool isIdentity, bool isNullable, bool isEnum) in tableColInfo)
            {
                // do not insert identity key values
                if (isIdentity && !includeIdentity)
                {
                    continue;
                }
                else if (identityOnly && !isIdentity)
                {
                    continue;
                }

                if (createParameters == true)
                {
                    yield return $"IDbDataParameter {name}Param = command.CreateParameter();";

                    yield return $"{name}Param.ParameterName = \"${name}\";";
                    yield return $"command.Parameters.Add({name}Param);";
                }

                if (instantiateParameters == true)
                {
                    if (isNullable == true)
                    {
                        if (isEnum)
                        {
                            yield return $"{name}Param.Value = (value.{name} == null) ? DBNull.Value : value.{name}.ToString();";
                        }
                        else
                        {
                            yield return $"{name}Param.Value = (value.{name} == null) ? DBNull.Value : value.{name};";
                        }
                    }
                    else
                    {
                        if (isEnum)
                        {
                            yield return $"{name}Param.Value = value.{name}.ToString();";
                        }
                        else
                        {
                            yield return $"{name}Param.Value = value.{name};";
                        }
                    }
                }

                if (createParameters == true)
                {
                    // add an empty line between parameters
                    yield return string.Empty;
                }
            }

            if (executeCommand == true)
            {
                if ((identityColName == null) || (!setIdentity) || (!isInsert))
                {
                    yield return "int rowsAffected = command.ExecuteNonQuery();";
                    yield return "if (rowsAffected == 0) throw new Exception(\"Command failed!\");";
                }
                else
                {
                    yield return "object? commandResult = command.ExecuteScalar();";
                    yield return "if (commandResult == null) throw new Exception(\"Command failed!\");";

                    switch (identityColType)
                    {
                        case "int":
                            yield return $"value.{identityColName} = Convert.ToInt32(commandResult);";
                            break;
                        case "long":
                            yield return $"value.{identityColName} = Convert.ToInt64(commandResult);";
                            break;
                        default:
                            yield return $"value.{identityColName} = ({identityColType})commandResult;";
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the SQL type for a given type.
    /// </summary>
    private static string getSqlType(string type, bool isEnum = false) => isEnum
        ? _sqliteTypeMap["enum"]
        : _sqliteTypeMap.TryGetValue(type, out string? name) ? name : "TEXT";

    private static Dictionary<string, string> _sqliteReadDirectives = new()
    {
        { "bool", "{0} = {1}.GetBoolean({2})" },
        { "byte", "{0} = {1}.GetByte({2})" },
        { "byte[]", "{0} = {1}.GetBytes({2})" },
        { "char", "{0} = {1}.GetChar({2})" },
        { "char[]", "{0} = {1}.GetChars({2})" },
        { "DateTime", "{0} = {1}.GetDateTime({2})" },
        { "DateTimeOffset", "{0} = new DateTimeOffset({1}.GetDateTime({2}))" },
        { "Decimal", "{0} = {1}.GetDecimal({2})" },
        { "double", "{0} = {1}.GetDouble({2})" },
        { "enum", "{0} = Enum.Parse<{3}>({1}.GetString({2}))" },
        { "float", "{0} = {1}.GetFloat({2})" },
        { "Guid", "{0} = {1}.GetGuid({2})" },
        { "short", "{0} = {1}.GetInt16({2})" },
        { "int", "{0} = {1}.GetInt32({2})" },
        { "long", "{0} = {1}.GetInt64({2})" },
        { "sbyte", "(sbyte){0} = {1}.GetByte({2})" },
        { "string", "{0} = {1}.GetString({2})" },
        { "TimeSpan", "{0} = {1}.GetTimeSpan({2})" },
        { "ushort", "(ushort){0} = {1}.GetInt16({2})" },
        { "uint", "(uint){0} = {1}.GetInt32({2})" },
        { "ulong", "(ulong){0} = {1}.GetInt64({2})" },
        { "Uri", "{0} = new Uri({1}.GetString({2}))" },
    };

    private static Dictionary<string, string> _sqliteNullableReadDirectives = new()
    {
        { "bool", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetBoolean({2})" },
        { "byte", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetByte({2})" },
        { "byte[]", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetBytes({2})" },
        { "char", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetChar({2})" },
        { "char[]", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetChars({2})" },
        { "DateTime", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetDateTime({2})" },
        { "DateTimeOffset", "{0} = {1}.IsDBNull({2}) ? null : new DateTimeOffset({1}.GetDateTime({2}))" },
        { "Decimal", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetDecimal({2})" },
        { "double", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetDouble({2})" },
        { "enum", "{0} = {1}.IsDBNull({2}) ? null : Enum.Parse<{3}>({1}.GetString({2}))" },
        { "float", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetFloat({2})" },
        { "Guid", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetGuid({2})" },
        { "short", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetInt16({2})" },
        { "int", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetInt32({2})" },
        { "long", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetInt64({2})" },
        { "sbyte", "(sbyte){0} = {1}.IsDBNull({2}) ? null : {1}.GetByte({2})" },
        { "string", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetString({2})" },
        { "TimeSpan", "{0} = {1}.IsDBNull({2}) ? null : {1}.GetTimeSpan({2})" },
        { "ushort", "(ushort){0} = {1}.IsDBNull({2}) ? null : {1}.GetInt16({2})" },
        { "uint", "(uint){0} = {1}.IsDBNull({2}) ? null : {1}.GetInt32({2})" },
        { "ulong", "(ulong){0} = {1}.IsDBNull({2}) ? null : {1}.GetInt64({2})" },
        { "Uri", "{0} = {1}.IsDBNull({2}) ? null : new Uri({1}.GetString({2}))" },
    };

    // Mapping pulled from https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types
    private static Dictionary<string, string> _sqliteTypeMap = new()
    {
        { "bool", "INTEGER" },
        { "byte", "INTEGER" },
        { "byte[]", "BLOB" },
        { "char", "TEXT" },
        { "char[]", "TEXT" },
        { "DateTime", "TEXT" },
        { "DateTimeOffset", "TEXT" },
        { "Decimal", "TEXT" },
        { "double", "REAL" },
        { "enum", "TEXT" },
        { "float", "REAL" },
        { "Guid", "TEXT" },
        { "short", "INTEGER" },
        { "int", "INTEGER" },
        { "long", "INTEGER" },
        { "sbyte", "INTEGER" },
        { "string", "TEXT" },
        { "TimeSpan", "TEXT" },
        { "ushort", "INTEGER" },
        { "uint", "INTEGER" },
        { "ulong", "INTEGER" },
        { "Uri", "TEXT" },
    };

}
