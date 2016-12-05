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
Run, C:\Program Files\Unity\Editor\Unity.exe, , , PID
WinWaitActive, ahk_pid %PID%
Sleep, 333
WinActivate, ahk_pid %PID%
Sleep, 333
Sleep, 7500
; username
Sleep, 100
; move to next input, password
Send, {Tab}
; password
Sleep, 100
; tab to submit
Send, {Tab}
Sleep, 100
Send, {Tab}
Sleep, 100
; submit!
Send, {Enter}
; wait and close
Sleep, 7500
WinClose, ahk_pid %PID%
Sleep, 333
Return

