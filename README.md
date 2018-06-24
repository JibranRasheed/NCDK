NCDK: The Chemistry Development Kit ported to C
===============================================

NCDK is .NET port of [the Chemistry Development Project (CDK)](https://github.com/cdk/cdk). Functionality is provided for many areas in cheminformatics.

NCDK is covered under LGPL v2.1. The modules are free and open-source and is easily integrated with other open-source or in-house projects.

The current release is based on [cdk 2018-03-06](https://github.com/cdk/cdk/tree/6fade7b5470669955835046d9fb09c48f658efde) snapshot.

Getting Started
---------------

Most of codes are written in C\#. You can learn NCDK from CDK documents, NCDKTests and NCDKDisplayTests project.

* NCDK --- Core module of NCDK
* NCDK.Display --- Depiction module of NCDK
* NCDKTests --- Unit tests for NCDK
* NCDKDisplayTests --- Unit tests for NCDK.Display
* MolViewer --- MOL file viewer, which can depict SMILES. It includes WPF control to show molecular/reaction.
* Documentation --- Sandcastle Help File Builder project

Build
-----

Windows: Open Developer Command Prompt for VS 2017, and launch "BuildAll.bat". Of course, you can build with Visual Studio 2017. Open NCDKDisplay.sln and build it.
Linux (.NET Core): To build NCDK.dll, launch "dotnet build --framework netstandard2.0" in NCDK directory.

Test
----

Windows: Launch "vstest.console.exe NCDKTests\bin\Release\netcoreapp2.1\NCDKTests.dll" and "vstest.console.exe NCDKDisplayTests\bin\Release\NCDKDisplayTests.dll".
Linux: Copy libinchi.so.#.## to output directory of NCDKTests, and make a libinchi.so link to it and launch "dotnet test" in NCDKTests directory.

NuGet Packages
--------------

* [NCDK](https://www.nuget.org/packages/NCDK/) --- .NET Standard 2.0 and .NET Framework 4.6.1. InChI features work on Windows or Linux.
* [NCDK.Display](https://www.nuget.org/packages/NCDK.Display/) --- .NET Framework 4.6.1.

Copyright (c) 2016-2018 Kazuya Ujihara
