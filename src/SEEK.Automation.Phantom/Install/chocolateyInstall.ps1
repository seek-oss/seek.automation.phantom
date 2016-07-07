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
    Environment Host {
      Name = "Messaging.SEEK.Automation.Simulator.Host"
      Value = "*"
    }

    Environment Port {
      Name = "Messaging.SEEK.Automation.Simulator.Port"
      Value = "80"
    }

    cUrlReservation ReservePort80 {
      Protocol = "http"
      Hostname = "*"
      Port = "80"
      User = "NT AUTHORITY\Network Service"
    }
    
    ForEach($port in 9000..9100) {
        cUrlReservation "ReservePort$port" {
          Protocol = "http"
          Hostname = "*"
          Port = "$port"
          User = "NT AUTHORITY\Network Service"
        }
    }

    cFirewallRule AllowInboundPort80 {
      Name = "SEEK.Automation.Simulator"
      Direction = "Inbound"
      LocalPort = "80"
      Protocol = "TCP"
      Action = "Allow"
    }

    cSelfHostedService Api {
      Name = "SEEK.Automation.Simulator"
      Executable = $(Join-Path $env:chocolateyPackageFolder "lib\SEEK.Automation.Simulator.exe")
      AutoStart = "false"
      Start = "false"
    }
  }
}

try
{

  $outputPath = $env:TEMP
  ApiConfiguration -Force -OutputPath $outputPath | Out-Null
  Start-DscConfiguration -Wait -Verbose -Path $outputPath -ErrorAction Stop
  Write-ChocolateySuccess 'SEEK Notifications - Microservice'

} catch {

  Write-ChocolateyFailure 'SEEK Notifications - Microservice' $($_.Exception.Message)
  $host.SetShouldExit(1)
  throw $_
  
}
