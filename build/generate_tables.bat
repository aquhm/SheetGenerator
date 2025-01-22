@echo off
setlocal enabledelayedexpansion

echo SheetGenerator Table Generation Tool
echo.

rem Set directories
set INSTALL_DIR=%USERPROFILE%\.sheetgenerator
set EXE_NAME=SheetGenerator.exe

echo Installation directory: %INSTALL_DIR%
echo.

rem Check if SheetGenerator is installed
if not exist "%INSTALL_DIR%\%EXE_NAME%" (
    echo Error: SheetGenerator is not installed.
    echo Please run install_windows.bat first to install SheetGenerator.
    pause
    exit /b 1
)

rem Check if configuration exists in installation directory
if not exist "%INSTALL_DIR%\settings.json" (
    echo Error: Configuration file not found in %INSTALL_DIR%
    echo Please ensure settings.json has been created through initialization.
    echo Try running SheetGenerator with --init flag first.
    pause
    exit /b 1
)

rem Process tables using the installed configuration
echo Processing tables...
cd /d "%INSTALL_DIR%"
"%INSTALL_DIR%\%EXE_NAME%" --loglevel Information --config "%INSTALL_DIR%\settings.json"

if %ERRORLEVEL% neq 0 (
    echo Error: Table generation failed (Exit code: %ERRORLEVEL%)
    pause
    exit /b 1
)

echo.
echo Table generation completed successfully!
echo Generated files can be found in: %INSTALL_DIR%\Generated
echo.

pause
