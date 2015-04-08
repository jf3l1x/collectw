$ErrorActionPreference = 'Stop';

$packageName = 'collectw.service'
$registryUninstallerKeyName = 'collectw.service' #ensure this is the value in the registry
$validExitCodes = @(0)
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$serviceName = "collectw"
$exePath = Join-Path $toolsdir "collectw.service.exe"

$existingService = Get-WmiObject -Class Win32_Service -Filter "Name='$serviceName'"

if ($existingService) 
{
  
  Stop-Service $serviceName
  "Waiting 3 seconds to allow existing service to stop."
  Start-Sleep -s 3

  $existingService.Delete()
  "Waiting 5 seconds to allow service to be uninstalled."
  Start-Sleep -s 5  
}

"Service Uninstalled"



