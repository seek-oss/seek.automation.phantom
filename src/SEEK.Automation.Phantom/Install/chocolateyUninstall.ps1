# This is a workaround for https://github.com/chocolatey/choco/issues/291
If ( -not ( Test-Path "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\PSDesiredStateConfiguration\PSDesiredStateConfiguration.Resource.psd1" ) ) {
  cmd /c mklink C:\Windows\System32\WindowsPowerShell\v1.0\Modules\PSDesiredStateConfiguration\PSDesiredStateConfiguration.Resource.psd1 C:\Windows\System32\WindowsPowerShell\v1.0\Modules\PSDesiredStateConfiguration\en-US\PSDesiredStateConfiguration.Resource.psd1
}

Configuration ApiConfiguration
{
  Import-DscResource -Module cNetworking
  Import-DscResource -Module cTopShelf

  Node 'localhost'
  {
    cSelfHostedService Api {
      Name = "SEEK.Automation.Simulator"
      Executable = $(Join-Path $env:chocolateyPackageFolder "lib\SEEK.Automation.Simulator.exe")
      Ensure = "Absent"
    }

    cUrlReservation ReleasePort {
      Protocol = "http"
      Hostname = "*"
      Port = "80"
      Ensure = "Absent"
    }

    cFirewallRule RemoveFirewallRule {
      Name = "SEEK.Automation.Simulator"
      Direction = "Inbound"
      LocalPort = "80"
      Protocol = "TCP"
      Action = "Allow"
      Ensure = "Absent"
    }

    Environment Host {
      Name = "Messaging.SEEK.Automation.Simulator.Host"
      Ensure = "Absent"
    }

    Environment Port {
      Name = "Messaging.SEEK.Automation.Simulator.Port"
      Ensure = "Absent"
    }
  }
}

try {

  $outputPath = $env:TEMP
  ApiConfiguration -Force -OutputPath $outputPath | Out-Null
  Start-DscConfiguration -Wait -Verbose -Path $outputPath -ErrorAction Continue
  Write-ChocolateySuccess 'SEEK Notifications - Service'

} catch {

  Write-ChocolateyFailure 'SEEK Notifications - Service' $($_.Exception.Message)
  throw $_
  
}
