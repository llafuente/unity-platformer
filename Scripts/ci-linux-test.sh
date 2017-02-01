#!/bin/sh

set -ex

/opt/Unity/Editor/Unity \
  -batchmode \
  -nographics \
  -projectPath $(pwd) \
  -runEditorTests \
  -editorTestsResultFile $(pwd)/unit-test-results.xml \
  -editorTestsFilter UnityPlatformer \
  -logFile $(pwd)/log-unit-test.txt
