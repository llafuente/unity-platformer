Write-Host "$(date) Start build script"-ForegroundColor green

Invoke-WebRequest "http://netstorage.unity3d.com/unity/01f4c123905a/Windows64EditorInstaller/UnitySetup64-5.4.3f1.exe" -OutFile .\UnitySetup64.exe

Start-Process -FilePath ".\UnitySetup64.exe" -Wait -ArgumentList ('/S', '/Q')

Write-Host "$(date) Unity Installed"-ForegroundColor green

Invoke-WebRequest "http://ahkscript.org/download/ahk-install.exe" -OutFile .\ahk-installer.exe

Start-Process -FilePath ".\ahk-installer.exe" -Wait -ArgumentList ('/S')

Write-Host "$(date) AHK Installed"-ForegroundColor green

Invoke-WebRequest "https://bitbucket.org/Unity-Technologies/unitytesttools/get/5.3.zip"  -OutFile .\UnityTestTools.zip

Expand-Archive .\UnityTestTools.zip .

Move-Item Unity-Technologies-unitytesttools-*\Assets\UnityTestTools C:\projects\unity-platformer\Assets\UnityTestTools

Write-Host "$(date) UnityTestTools Installed"-ForegroundColor green

$password = $env:UNITY_PASSWORD

$password = $password -split ''

$pass_commands = "Send, {" + ($password[1..($password.Length-2)] -join "}`nSleep, 100`nSend, {") + "}`n"

$username = $env:UNITY_USERNAME

$username = $username -split ''

$user_commands = "Send, {" + ($username[1..($username.Length-2)] -join "}`nSleep, 100`nSend, {") + "}`n"

(Get-Content C:\projects\unity-platformer\Scripts\unity.ahk).replace('; username', $user_commands).replace('; password', $pass_commands) | Set-Content C:\projects\unity-platformer\unity.ahk

$env:Path += ";C:\Program Files\AutoHotkey"

Start-Process -FilePath "AutoHotkey.exe" -Wait -ArgumentList ('C:\projects\unity-platformer\unity.ahk')

Write-Host "$(date) Unity login bypassed"-ForegroundColor green
