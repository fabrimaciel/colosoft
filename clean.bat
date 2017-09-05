@echo off

call defines.bat

echo 'Clean...'

RMDIR %outputbinaries% /s /q

call buildproject.cmd ".\Analysis\CodeAnalysisFix\CodeAnalysisFix.sln"
CodeAnalysisFix.exe