#!/bin/sh

/usr/bin/time -v /opt/Unity/Editor/Unity \
  -batchmode \
  -nographics \
  -projectPath $(pwd) \
  -executeMethod UnityTest.Batch.RunUnitTests \
  -resultFilePath=$(pwd)/results.xml \


#-executeMethod UnityTest.Batch.RunIntegrationTests
#-testscenes=scene1,scene2
#-targetPlatform=StandaloneWindows
#-resultsFileDirectory=C:\temp
