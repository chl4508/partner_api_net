@echo off

IF "%1" == "" (
	goto :func_all
)ELSE (
	IF "%1"=="all" (
		goto :func_all
	)ELSE (
		GOTO :func_else
	)
)

:func_all
call "%0" meta bat
call "%0" space bat
call "%0" billing bat
call "%0" meelib bat
goto QUIT

:func_else
mkdir "openapi"
dotnet "tool" "restore"
dotnet "build"
set projectTarget=%1
set projectPath=.\dotnet\Service\%projectTarget%\%projectTarget%Apis\%projectTarget%Apis.csproj
set projectDllPath=.\dotnet\Service\%projectTarget%\%projectTarget%Apis\bin\Debug\net6.0\Colorverse.%projectTarget%Apis.dll
set swaggerPath=openapi\%projectTarget%.json
dotnet "tool" "run" "swagger" "tofile" "--output" "%swaggerPath%" "%projectDllPath%" "v1"
goto QUIT

:QUIT