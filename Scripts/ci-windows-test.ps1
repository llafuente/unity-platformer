Write-Host "$(date) Start test script"-ForegroundColor green

$env:Path += ";C:\Program Files\Unity\Editor"

Start-Process -FilePath "Unity.exe" -Wait -ArgumentList ('-batchmode', '-nographics', '-projectPath', 'C:\projects\unity-platformer', '-runEditorTests', '-editorTestsResultFile', 'C:\projects\unity-platformer\unit-test-results.xml', '-editorTestsFilter', 'UnityPlatformer', '-logFile', 'C:\projects\unity-platformer\log-unit-test.txt', '-quit')

Write-Host "$(date) Upload result"-ForegroundColor green
