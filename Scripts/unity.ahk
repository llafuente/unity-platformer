; This script was created using Pulover's Macro Creator
; www.macrocreator.com

#NoEnv
SetWorkingDir %A_ScriptDir%
CoordMode, Mouse, Window
SendMode Input
#SingleInstance Force
SetTitleMatchMode 2
#WinActivateForce
SetControlDelay 1
SetWinDelay 0
SetKeyDelay -1
SetMouseDelay -1
SetBatchLines -1


Macro1:
LOGFile = C:\projects\unity-platformer\autohotkey.txt
Run, C:\Program Files\Unity\Editor\Unity.exe, , , PID
Sleep, 15000
; App window in CI never get active...
FileAppend, Waited for the window to start, %LOGFile%
; A window's title can contain WinTitle anywhere inside it to be a match.
SetTitleMatchMode 2
; username
Sleep, 100
; move to next input, password
ControlSend, , {Tab}, Unity
; password
Sleep, 100
FileAppend, typed username and password, %LOGFile%
; tab to submit
ControlSend, , {Tab}, Unity
Sleep, 100
ControlSend, , {Tab}, Unity
Sleep, 100
; submit!
ControlSend, , {Enter}, Unity
; wait and close
Sleep, 7500
FileAppend, Choose Unity personal, %LOGFile%
;CoordMode, Mouse, Window
ControlClick, x700 y290, Unity ; Unity personal
ControlClick, x700 y430, Unity ; Next
Sleep, 2500
ControlClick, x268 y346, Unity ; I don't use Unity in a professional capacity
ControlClick, x700 y430, Unity ; Next
Sleep, 2500
FileAppend, Start Using Unity, %LOGFile%
ControlClick, x500 y390, Unity ; Start Using Unity
Sleep, 2500
WinClose, ahk_pid %PID%
FileAppend, Close window, %LOGFile%
Sleep, 333
Return
