#!/usr/bin/env pwsh
# Add `--filter *.METHOD*` or similar to run subset of benchmarks
dotnet run -c Release --project NCsvPerf\NCsvPerf.csproj -- -m --warmupCount 1 --minIterationCount 2 --maxIterationCount 5 --iterationTime 2000
