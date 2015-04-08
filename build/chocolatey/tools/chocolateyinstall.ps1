$ErrorActionPreference = 'Stop';

$packageName = 'collectw.service' # arbitrary name for the package, used in messages
$validExitCodes = @(0) #please insert other valid exit codes here, exit codes for ms http://msdn.microsoft.com/en-us/library/aa368542(VS.85).aspx
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$psFile = Join-Path $toolsdir "Install-Service.ps1"
Install-ChocolateyPowershellCommand $packageName $psFile

