@echo off
cd %~dp0

powershell -Command "& {Import-Module .\packages\psake.4.3.2\tools\psake.psm1; Invoke-psake .\scripts\default.ps1 %1 -parameters @{"buildConfiguration"='Release';} }" 
