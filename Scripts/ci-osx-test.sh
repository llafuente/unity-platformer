#!/bin/sh

set -ex

/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -projectPath $(pwd) \
  -runEditorTests \
  -editorTestsResultFile $(pwd)/unit-test-results.xml \
  -editorTestsFilter UnityPlatformer \
  -logFile $(pwd)/log-unit-test.txt

cat $(pwd)/log-unit-test.txt
cat $(pwd)/unit-test-results.xml
