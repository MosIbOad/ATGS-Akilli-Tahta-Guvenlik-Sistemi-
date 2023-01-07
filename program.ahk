#NoTrayIcon

WS_EX_TOOLWINDOW := 0x00000080
Loop
{
   WinSet, ExStyle, +%WS_EX_TOOLWINDOW%, ahk_exe HDBS.exe
   Sleep,100
}

Pause::Suspend