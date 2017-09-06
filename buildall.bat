@echo off

call defines.bat

echo Building %1...

if "%1" == "" goto build

echo 123
:config
set configuration=%1

:build

echo ------------------------------------------------------------------------
echo Build Core Projects
echo ------------------------------------------------------------------------

echo call buildproject.cmd ".\Contributions\MefContrib.Models.Provider\MefContrib.Models.Provider.sln"
call buildproject.cmd ".\Core\Core.sln"
call buildproject.cmd ".\Mef\Mef.sln"
call buildproject.cmd ".\Validation\Validation.sln"
call buildproject.cmd ".\Text\Text.sln"
call buildproject.cmd ".\Query\Query.sln"
call buildproject.cmd ".\Data\Data.sln"
call buildproject.cmd ".\Business\Business.sln"
call buildproject.cmd ".\Data\Schema\Schema.sln"
call buildproject.cmd ".\Caching\Caching.sln"
call buildproject.cmd ".\Data\Caching\Data.Caching.sln"
call buildproject.cmd ".\DataAccess\DataAccess.sln"
call buildproject.cmd ".\Security\Security.sln"
call buildproject.cmd ".\Drawing\Drawing.sln"
call buildproject.cmd ".\Reports\Reports.sln"
call buildproject.cmd ".\Web\Web.sln"
call buildproject.cmd ".\Net\Net.sln"
call buildproject.cmd ".\Data\Caching\Remote\Data.Caching.Remote.sln"
call buildproject.cmd ".\ServiceProcess\ServiceProcess.sln"
call buildproject.cmd ".\Security\Remote\Remote.sln"
call buildproject.cmd ".\Security\Remote\Remote.sln"
call buildproject.cmd ".\Kendo\Kendo.sln"
call buildproject.cmd ".\Net\Net.Remote.sln"
call buildproject.cmd ".\Excel\Excel.sln"
call buildproject.cmd ".\GDA\GDA.Provider.Oracle.sln"

call buildproject.cmd ".\Query\Database\Query.Database.sln"
call buildproject.cmd ".\Data\Database\Database.sln"


:build_sl

:writePublicKeyToken

echo -------------------------------
echo All done

set Cfg=%1
if %Cfg%_ ==_set Cfg=Pause
if %Cfg%_==/silent_ goto end
pause

:end
