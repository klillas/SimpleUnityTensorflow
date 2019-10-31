IF NOT EXIST ./csharp/ (
    mkdir csharp
) 

IF NOT EXIST ../Python/protobuf/ (
    mkdir ..\Python\protobuf
) 

..\..\Packages\Google.Protobuf.Tools.3.10.1\tools\windows_x64\protoc.exe --csharp_out=.\csharp\ --python_out=..\python\protobuf\ Telegrams.proto