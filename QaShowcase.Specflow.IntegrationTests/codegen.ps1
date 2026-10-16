# Requires QA_BASE_URL and QA_TENANT environment variables
$baseUrl = $env:QA_BASE_URL
$tenant = $env:QA_TENANT

if (-not $baseUrl -or -not $tenant) {
    Write-Error "QA_BASE_URL and QA_TENANT environment variables must be set"
    exit 1
}

bin/Debug/net6.0/playwright.ps1 codegen "$baseUrl/$tenant"