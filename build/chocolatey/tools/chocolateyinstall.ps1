$ErrorActionPreference = 'Stop';

$packageName = 'collectw.service' # arbitrary name for the package, used in messages
$validExitCodes = @(0) #please insert other valid exit codes here, exit codes for ms http://msdn.microsoft.com/en-us/library/aa368542(VS.85).aspx
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$psFile = Join-Path $toolsdir "Install-Service.ps1"
$serviceName = "collectw"
$exePath = Join-Path $toolsdir "collectw.service.exe"

$existingService = Get-WmiObject -Class Win32_Service -Filter "Name='$serviceName'"

if ($existingService) 
{
  "'$serviceName' exists already. Stopping."
  Stop-Service $serviceName
  "Waiting 3 seconds to allow existing service to stop."
  Start-Sleep -s 3

  $existingService.Delete()
  "Waiting 5 seconds to allow service to be uninstalled."
  Start-Sleep -s 5  
}

"Installing the service."
New-Service -BinaryPathName $exePath -Name $serviceName -DisplayName $serviceName -Description "Performance counters collector and forwarder" -StartupType Automatic 
"Service was installed sucessfully."


