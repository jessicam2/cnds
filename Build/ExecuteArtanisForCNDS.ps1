$currentDirectory = (Get-Item -Path ".\" -Verbose).parent.FullName
cd Artanis

cmd.exe /c Lpp.Artanis.exe /Interface=true /Path=$currentDirectory /Config=CNDSInterfaces
cmd.exe /c Lpp.Artanis.exe /ViewModel=true /Path=$currentDirectory /Config=CNDSViewModels
cmd.exe /c Lpp.Artanis.exe /NetClient=true /Path=$currentDirectory /Config=CNDSNetClient

cd ..

#copy the CNDS API interfaces and view models to the PMN Api scripts folder so that the portal can consume them.
Copy-Item ..\Lpp.CNDS.Api\Scripts\Lpp.CNDS.* ..\Lpp.Dns.Api\Scripts