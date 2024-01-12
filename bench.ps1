#!/usr/bin/env pwsh
# Add `--filter *.METHOD*` or similar to run subset of benchmarks

[CmdletBinding()]
param (
    [Parameter(Mandatory = $false)]
    [string]$Filter = "*",

    [Parameter(Mandatory = $false)]
    [switch]$UseServerGC
)

$project = Join-Path $PSScriptRoot "NCsvPerf/NCsvPerf.csproj"
dotnet build $project -c Release "/p:ServerGarbageCollection=$UseServerGC"
dotnet run --project $project -c Release --no-build -- -m --warmupCount 1 --minIterationCount 2 --maxIterationCount 5 --iterationTime 2000 --filter $Filter
