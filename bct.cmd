@echo off
setlocal
if "%ANDREW_OPTIMIZED%"=="1" (
  set ANDREW_ARCH_PART=release
) else (
  set ANDREW_ARCH_PART=
)
pushd src\tests
call build.cmd crossgen2 %ANDREW_ARCH_PART%
popd
endlocal
