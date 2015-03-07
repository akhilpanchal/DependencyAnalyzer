:run.bat
:
:runs dependency analyzer code

start ./Server\bin\debug\Server.exe "http://localhost:4001/ServerService"
start ./Server2\bin\debug\Server.exe "http://localhost:4002/ServerService2"
start ./Client\bin\debug\Client_GUI.exe
start ./Client2\bin\debug\Client_GUI.exe

