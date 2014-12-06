# FRepl - A REPL Library for F Sharp

## Introduction

### Summary

FRepl is an F Sharp (.NET) library providing REPL (Read-Evaluate-Print-Loop) functionality. It's written in F Sharp and targets F Sharp, but since it takes the form of a .NET library, it should be callable from any .NET-aware langauge.

## Building FRepl

### Building in Visual Studio 2013
Open FRepl.sln in Visual Studio 2013, choose Build->Build Solution. This will build the FRepl library project and the Example projects. This has been tested in Visual Studio Express 2013.
The build outputs (assemblies) go in $(SolutionDir)bin\Debug (for Debug builds) and $(SolutionDir)bin\Release (for Release builds).

### Building in other versions of Visual Studio
I haven't tested it, but opening FRepl.sln and choosing Build->Build Solution will probably work with versions from Visual Studio 2010 onwards. You may need to install F# Build tools to get it to work.

### Building with Microsoft build tools without Visual Studio
Your best bet is to run MSBuild.exe directly on FRepl.sln. You will need F Sharp Development tools (fsc.exe etc.) installed.

### Building on other platforms.
Best of luck!




