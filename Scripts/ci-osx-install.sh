#! /bin/sh
# to update the hashes go to:
# STABLE: BASE_URL=http://netstorage.unity3d.com/unity at https://unity3d.com/get-unity/download/archive
# BETA: BASE_URL=http://beta.unity3d.com/download at https://unity3d.com/unity/beta/archive
# check the drop down and copy those links :)

# latest 5.3: c347874230fb | 5.3.7f1
# latest 5.4: 01f4c123905a | 5.4.3f1

set -x

# we are going to check against the "lastest" 5.5
BASE_URL=http://beta.unity3d.com/download
HASH=03364468e96e
VERSION=5.5.0b11
UNITY_TEST_TOOL_BRANCH=5.5

download() {
  file=$1
  url="$BASE_URL/$HASH/$package"

  echo "Downloading from $url: "
  curl -o `basename "$package"` "$url"
}

install() {
  package=$1
  download "$package"

  echo "Installing "`basename "$package"`
  sudo installer -dumplog -package `basename "$package"` -target /
}

# See $BASE_URL/$HASH/unity-$VERSION-$PLATFORM.ini for complete list
# of available packages, where PLATFORM is `osx` or `win`
# curl http://netstorage.unity3d.com/unity/${HASH}/unity-{$VERSION}-osx.ini

install "MacEditorInstaller/Unity-$VERSION.pkg"

# @llafuente: I'm not going to build just test
#install "MacEditorTargetInstaller/UnitySetup-Windows-Support-for-Editor-$VERSION.pkg"
#install "MacEditorTargetInstaller/UnitySetup-Mac-Support-for-Editor-$VERSION.pkg"
#install "MacEditorTargetInstaller/UnitySetup-Linux-Support-for-Editor-$VERSION.pkg"

curl -o "UnityTestTools.tar.bz2" "https://bitbucket.org/Unity-Technologies/unitytesttools/get/${UNITY_TEST_TOOL_BRANCH}.tar.bz2"
UNITY_TEST_TOOLS_PATH=$(tar xvjf "UnityTestTools.tar.bz2" 2>&1 | tail -n 1 | cut -c3- | cut -d'/' -f1)
mv "${UNITY_TEST_TOOLS_PATH}/Assets/UnityTestTools" "./Assets/UnityTestTools"
#dev
#rm UnityTestTools.tar.bz2
#rm -rf Unity-Technologies-unitytesttools-*/
