@echo off
mkdir Build
"C:\Program Files\Unity\Editor\Unity.exe" -batchmode -nographics  -projectPath %cd%  -editorTestsFilter UnityPlatformer -runEditorTests -logFile %cd%\Build\log-unit-test.txt -editorTestsResultFile %cd%\Build\unit-test-results.xml
