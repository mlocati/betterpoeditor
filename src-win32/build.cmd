@ECHO off
REM echo.exe from http://www.paulsadowski.com/WSH/cmdprogs.htm
SETLOCAL
SET ECHO="%~dp0echo.exe"
%ECHO% -n "Initializing... "
SET FWORK_VERSION=2.0.50727
SET BUILDER="%SystemRoot%\Microsoft.NET\Framework\v%FWORK_VERSION%\MSBuild.exe"
SET MYFOLDER=%~dp0
SET BASEFOLDER=%~dp0../
SET LOGFILE="%MYFOLDER%build.log"
%ECHO% "done."
REM "%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe" /nologo /p:Configuration=Release ../src/BetterPoEditor.sln

%ECHO% -n "Building Jayrock.Json... "
CD "%BASEFOLDER%src/Jayrock.Json"
%BUILDER% /nologo /p:Configuration=Release Jayrock.Json.csproj >%LOGFILE% 2>&1
IF ERRORLEVEL 1 GOTO SeeBuildLon
%ECHO% "done."

%ECHO% -n "Building NetSpell.SpellChecker... "
CD "%BASEFOLDER%src/NetSpell.SpellChecker"
%BUILDER% /nologo /p:Configuration=Release NetSpell.SpellChecker.csproj >%LOGFILE% 2>&1
IF ERRORLEVEL 1 GOTO SeeBuildLon
%ECHO% "done."

%ECHO% -n "Building NetSpell.DictionaryBuild... "
CD "%BASEFOLDER%src/NetSpell.DictionaryBuild"
%BUILDER% /nologo /p:Configuration=Release NetSpell.DictionaryBuild.csproj >%LOGFILE% 2>&1
IF ERRORLEVEL 1 GOTO SeeBuildLon
%ECHO% "done."

%ECHO% -n "Building TidyNet... "
CD "%BASEFOLDER%src/TidyNet"
%BUILDER% /nologo /p:Configuration=Release TidyNet.csproj >%LOGFILE% 2>&1
IF ERRORLEVEL 1 GOTO SeeBuildLon
%ECHO% "done."

%ECHO% -n "Building BetterPoEditor... "
CD "%BASEFOLDER%src/BetterPoEditor"
%BUILDER% /nologo /p:Configuration=Release BetterPoEditor.csproj >%LOGFILE% 2>&1
IF ERRORLEVEL 1 GOTO SeeBuildLon
%ECHO% "done."

DEL %LOGFILE%
CD "%MYFOLDER%"

%ECHO% ""
%ECHO% "Everything has been prepared. Now compile the setup with InnoSetup using the\ncreate_setup.iss file."
%ECHO% ""

GOTO End

:SeeBuildLon
%ECHO% "ERROR! See build.log for details."
GOTO End

:End
ENDLOCAL
PAUSE