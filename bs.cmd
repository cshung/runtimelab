@echo off
setlocal
if "%ANDREW_OPTIMIZED%"=="1" (
  set ANDREW_ARCH_PART= -c release
) else (
  set ANDREW_ARCH_PART=
)
pushd src\tests\JIT\Regression\JitBlue\Runtime_66089\
call %~dp0dotnet.cmd build %ANDREW_ARCH_PART%
popd
endlocal
