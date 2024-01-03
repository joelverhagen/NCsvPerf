#!/usr/bin/env pwsh
# Add `--filter *.METHOD*` or similar to run subset of benchmarks
dotnet run -c Release --project NCsvPerf\NCsvPerf.csproj -- -m --minWarmupCount 3 --maxWarmupCount 5 --minIterationCount 5 --maxIterationCount 11
