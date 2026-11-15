$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

dotnet build "$root\src\UnlockAllCharacters" -c Release -v q --nologo
if ($LASTEXITCODE -ne 0) { throw "dotnet build 失败" }

$out = Join-Path $root 'dist\UnlockAllCharacters'
New-Item -ItemType Directory -Force $out | Out-Null
Copy-Item "$root\src\UnlockAllCharacters\bin\Release\netstandard2.0\UnlockAllCharacters.dll" $out -Force
Copy-Item "$root\src\UnlockAllCharacters\UnlockAllCharacters.json" $out -Force
Copy-Item "$root\src\UnlockAllCharacters\icon.png" $out -Force
Write-Host "OK -> $out"
Get-ChildItem $out | Select-Object Name, Length | Format-Table -AutoSize
