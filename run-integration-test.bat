@echo off
mkdir Build
"C:\Program Files\Unity\Editor\Unity.exe" -batchmode -nographics -projectPath %cd% -executeMethod UnityTest.Batch.RunIntegrationTests -logFile %cd%\Build\log.txt -testscenes=LaddersTest -resultsFileDirectory= %cd%\Build\
