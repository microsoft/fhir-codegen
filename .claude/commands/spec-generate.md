# Specification Generator

Think deeply about this command to generate comprehensive specification documents for functions and classes.

## Usage
```
/spec-generate <target> [--depth=5] [--max-functions=50] [--exclude-external] [--include-interfaces]
```

## Command Logic

I'll parse the command arguments and execute the specification generation process.

Let me start by extracting and parsing the target and options from your command:

```
Target: {args[0] if args else "NOT_PROVIDED"}
Depth: {next((arg.split('=')[1] for arg in args if arg.startswith('--depth=')), '5')}
Max Functions: {next((arg.split('=')[1] for arg in args if arg.startswith('--max-functions=')), '50')}
Exclude External: {'Yes' if '--exclude-external' in args else 'No'}
Include Interfaces: {'Yes' if '--include-interfaces' in args else 'No'}
```

Now I'll execute the analysis in phases:

## Phase 1: Target Discovery

I'll search for the target in the codebase and identify its location and basic structure.

## Phase 2: Dependency Mapping

I'll use Task agents to analyze the target's dependencies and called functions in parallel.

## Phase 3: Code Analysis

I'll perform detailed analysis of the target's implementation, logic flow, and data transformations.

## Phase 4: Document Generation

I'll synthesize all findings into a comprehensive specification document.

---

**Ready to begin analysis. Please specify your target, for example:**
- `/spec-generate PackageLoader`
- `/spec-generate CSharpFirely2.Export --depth=2`

I'll create a detailed specification document saved as `specs/{target}-specification.md`.

---

## Implementation Notes

This command will:

1. **Parse Arguments**: Extract target name and configuration options
2. **Locate Target**: Use Grep and Glob to find the function/class definition
3. **Analyze Dependencies**: Spawn Task agents for parallel analysis of called functions
4. **Generate Diagrams**: Create Mermaid flowcharts showing the execution flow
5. **Create Specification**: Compile comprehensive markdown documentation

The output follows this structure:

```markdown
# {Target} Specification

## Executive Summary
Brief overview of the component's purpose and functionality

## Architecture Overview  
How it fits into the larger system

## Detailed Algorithm
Step-by-step breakdown of the logic

## Mermaid Workflow Diagram
Visual representation of the process flow

## Dependencies & Interactions
Functions called and callers of this function

## Data Models
Input/output structures and transformations

## Error Handling
Exception paths and error scenarios

## Performance Considerations
Complexity analysis and optimization notes
```

Each section will be populated with detailed analysis based on the actual code implementation.
