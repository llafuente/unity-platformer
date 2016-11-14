#!/bin/sh

mkdir Build

/usr/bin/time -v /opt/Unity/Editor/Unity \
  -batchmode \
  -nographics \
  -projectPath $(pwd) \
  -editorTestsFilter UnityPlatformer -runEditorTests -editorTestsResultFile \
  -editorTestsResultFile=$(pwd)/Build/unit-test-results.xml \

#-executeMethod UnityTest.Batch.RunIntegrationTests
#-testscenes=scene1,scene2
#-targetPlatform=StandaloneWindows
#-resultsFileDirectory=C:\temp
