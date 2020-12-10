#!/bin/bash
set -e
nuget restore giza.sln
msbuild /p:Configuration=Debug giza.sln
mono ./packages/NUnit.ConsoleRunner.3.11.1/tools/nunit3-console.exe \
    ./MetaphysicsIndustries.Giza.Test/bin/Debug/MetaphysicsIndustries.Giza.Test.dll
