if exist %1 goto build
echo Not found %1 - Skipped! 
goto end

:build

echo %configuration%
echo Building %1...

if "%2" == "" goto outundefined
%msbuild% %1 /nologo /verbosity:quiet /t:Build /toolsversion:4.0 /p:Configuration=%configuration%;Plataform="Any CPU" /p:OutputPath="%2"
echo Done %1
del "%2\*.pdb

if ERRORLEVEL 1 goto showerror
goto end

:outundefined
%msbuild% %1 /nologo /verbosity:quiet /t:Build /toolsversion:4.0 /p:Configuration=%configuration%;Plataform="Any CPU"
echo Done %1

if ERRORLEVEL 1 goto showerror
goto end

:showerror
echo ---------------------------------------------
echo Fail on build
echo ---------------------------------------------

pause

:end