@echo off
setlocal enabledelayedexpansion

echo SheetGenerator Installer
echo.

rem Set directories
set INSTALL_DIR=%USERPROFILE%\.sheetgenerator
set EXE_NAME=SheetGenerator.exe
set SCRIPT_DIR=%~dp0
set PROJECT_DIR=%SCRIPT_DIR%\..\SheetGenerator
set BUILD_DIR=%SCRIPT_DIR%\..\bin

echo Installation path: %INSTALL_DIR%
echo.

rem Check .NET SDK installation
where dotnet >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo Error: .NET SDK is not installed.
    echo Please install .NET SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

rem Create installation directory
if not exist "%INSTALL_DIR%" (
    mkdir "%INSTALL_DIR%"
    echo Created installation directory: %INSTALL_DIR%
)

rem Clean build directory
if exist "%BUILD_DIR%" (
    echo Cleaning build directory...
    rd /s /q "%BUILD_DIR%"
)

rem Build application
echo Building application...
dotnet publish "%PROJECT_DIR%\SheetGenerator.csproj" ^
    --output "%BUILD_DIR%" ^
    --runtime win-x64 ^
    -p:PublishSingleFile=true ^
    -p:EnableCompressionInSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:DebugType=None ^
    -p:DebugSymbols=false ^
    -c Release

if %ERRORLEVEL% neq 0 (
    echo Error: Build failed
    pause
    exit /b 1
)

rem Copy executable
echo.
echo Copying executable to: %INSTALL_DIR%
copy /Y "%BUILD_DIR%\%EXE_NAME%" "%INSTALL_DIR%"
if %ERRORLEVEL% neq 0 (
    echo Error: Failed to copy executable
    pause
    exit /b 1
)

rem Initialize SheetGenerator
echo.
echo Initializing SheetGenerator...
cd %SCRIPT_DIR%
"%INSTALL_DIR%\%EXE_NAME%" --init --loglevel debug

if %ERRORLEVEL% neq 0 (
    echo Error: Initialization failed (Exit code: %ERRORLEVEL%)
    pause
    exit /b 1
)

echo.
echo Installation and initialization completed successfully!
echo Executable location: %INSTALL_DIR%\%EXE_NAME%
echo.

pause
