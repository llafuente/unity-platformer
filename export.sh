#!/bin/sh

# export package
/opt/Unity/Editor/Unity \
  -batchmode \
  -nographics \
  -projectPath $(pwd) \
  -exportPackage "Assets/UnityPlatformer" unity-platformer.unitypackage \
  -quit
