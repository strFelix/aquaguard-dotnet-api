param(
    [string]$StagingUrl = "https://aquaguardapi-staging-e7aeezcqatemaght.canadacentral-01.azurewebsites.net",
    [string]$ProductionUrl = "https://aquaguardapi-evh6dzbvg4b8d7cd.canadacentral-01.azurewebsites.net"
)

$environments = @(
    @{ Name = "Staging"; Url = $StagingUrl },
    @{ Name = "Production"; Url = $ProductionUrl }
)

foreach ($environment in $environments) {
    $healthUrl = "$($environment.Url.TrimEnd('/'))/health"
    $response = Invoke-WebRequest -Uri $healthUrl -Method Get -UseBasicParsing

    if ($response.StatusCode -ne 200 -or $response.Content.Trim() -ne "Healthy") {
        throw "$($environment.Name) health check failed: HTTP $($response.StatusCode), response '$($response.Content.Trim())'."
    }

    Write-Output "$($environment.Name): Healthy ($healthUrl)"
}
