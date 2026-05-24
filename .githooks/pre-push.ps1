$ErrorActionPreference = 'Stop'

Write-Host 'Running dotnet-backend-snippets pre-push checks...'

function Invoke-Checked {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$FilePath $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

$hasNet8Runtime = dotnet --list-runtimes | Select-String -Pattern 'Microsoft\.NETCore\.App 8\.'
if (-not $hasNet8Runtime -and -not $env:DOTNET_ROLL_FORWARD) {
    Write-Host 'No .NET 8 runtime was found. Using DOTNET_ROLL_FORWARD=Major for this local hook run.'
    $env:DOTNET_ROLL_FORWARD = 'Major'
}

Invoke-Checked -FilePath 'dotnet' -Arguments @('restore', '-p:NuGetAudit=false')
Invoke-Checked -FilePath 'dotnet' -Arguments @('build', '--no-restore', '--configuration', 'Release', '-p:NuGetAudit=false')
Invoke-Checked -FilePath 'dotnet' -Arguments @('test', '--no-build', '--configuration', 'Release')
Invoke-Checked -FilePath 'dotnet' -Arguments @('format', '--verify-no-changes', '--no-restore')
Invoke-Checked -FilePath 'python' -Arguments @('scripts/check_markdown_links.py')
