To upgrade/restore workloads:
dotnet workload install maui

List:
dotnet workload list

To Fix MAUI environment:
https://github.com/Redth/dotnet-maui-check

To enforce dotNET SDK use:
dotnet new globaljson --sdk-version 6.0.111 --roll-forward major

where the 6.0.111 comes from "dotnet --version"
