setlocal
del COMPLUS*
set COMPLus_LogEnable=1
set COMPLus_LogFacility=0x00080000
set COMPLus_LogLevel=A
set COMPLus_LogToFile=1
set COMPLus_LogWithPid=1
c:\toolssw\debuggers\amd64\cdb.exe -c $$^>a^<debugger.script c:\toolssw\debuggers\amd64\cdb.exe -c $$^>a^<debuggee.script "C:\dev\runtime\artifacts\tests\coreclr\Windows.x64.Debug\Tests\Core_Root\corerun.exe" greenthread_delay.dll > log.txt
c:\toolssw\slack\alert "Done debug logging"
endlocal