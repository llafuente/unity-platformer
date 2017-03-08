#!/bin/sh

set -ex

UNITY_TEST_TOOL_BRANCH=5.5

# try to solve some dpkg errors
sudo apt-get clean
sudo apt-get update && sudo apt-get upgrade

wget -O unity-editor.deb 'http://beta.unity3d.com/download/59c25c92588f/unity-editor_amd64-5.5.0xp1Linux.deb'
# install and fix dependencies broken by installing Unity
# in the same command, because dpkg will fail...
sudo dpkg -i unity-editor.deb || sudo apt-get -y -f install


curl -o "UnityTestTools.tar.bz2" "https://bitbucket.org/Unity-Technologies/unitytesttools/get/${UNITY_TEST_TOOL_BRANCH}.tar.bz2"
UNITY_TEST_TOOLS_PATH=$(tar xvjf "UnityTestTools.tar.bz2" 2>&1 | tail -n 1 | cut -c1- | cut -d'/' -f1)
mv "${UNITY_TEST_TOOLS_PATH}/Assets/UnityTestTools" "./Assets/UnityTestTools"
