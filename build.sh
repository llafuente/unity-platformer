#!/bin/sh

mkdir -p "$(pwd)/build/linux"
echo > $(pwd)/build/build.log

# /usr/bin/xvfb-run -nographics -executeMethod $2 -resultFilePath=$3 -logFile
# -buildOSXUniversalPlayer "$(pwd)/build/osx" \
# -buildWindowsPlayer "$(pwd)/build/win" \
# -buildLinuxUniversalPlayer "$(pwd)/build/linux" \

/opt/Unity/Editor/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/build/build.log \
  -createProject $(pwd) \
  -buildLinuxUniversalPlayer "$(pwd)/build/linux" \
  -quit

RESULT=$?

if [ "${RESULT}" = "0" ] ; then
  echo -e "\n\033[36;1mBuild Success (${RESULT})\033[0m\n"
  exit 0
else
  echo -e "\n\033[31;1mBuild Failed (${RESULT})\033[0m\n"

  echo $(pwd)/build/build.log
  cat $(pwd)/build/build.log
  exit 1
fi
