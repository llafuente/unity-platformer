#!/bin/sh

mkdir -p Build

/usr/bin/time -v /opt/Unity/Editor/Unity \
  -batchmode \
  -nographics \
  -projectPath $(pwd) \
  -runEditorTests \
  -editorTestsResultFile $(pwd)/Build/unit-test-results.xml \
  -editorTestsFilter UnityPlatformer \
  -logFile $(pwd)/Build/log-unit-test.txt

#-executeMethod UnityTest.Batch.RunIntegrationTests
#-testscenes=scene1,scene2
#-targetPlatform=StandaloneWindows
#-resultsFileDirectory=C:\temp
