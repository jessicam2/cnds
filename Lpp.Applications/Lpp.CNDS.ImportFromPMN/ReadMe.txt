/**** Usage Instructions for Lpp.CNDS.ImportFromPMN ****/
This application is intended as a one time import from PopMedNet to CNDS.

When run it will:
a) Delete from CNDS all entities associated with the network specified in the application configuration.
b) Import the specified Network based on the NetworkID in the application configuration.
c) Import all active Organizations from PMN - active is defined as Deleted == false.
d) Import all active Users from PMN - active is defined as Deleted == false, IsActive == 2 (the user needs to have been activiated and associated with an Organization), and UserType == "User" (background service accounts used by PMN are not imported into CNDS).
e) Import all active DataMarts from PMN - active is defined as Deleted == false, the parent Organization is not deleted, and the DataMart is not a "Local" datamart used for PMN metadata queries.


Configuration:
1. Confirm the connection strings are correct in the ConnectionStrings.config. There should be two connection string entries: for CNDS with the name of "DataContext", for PMN with the name of "PMN".

2. Confirm the NetworkID specified in the Lpp.CNDS.ImportFromPMN.exe.config is the correct NetworkID from PMN. Confirm that the NetworkID is unique and has not already been used in an import before. If it has change in PMN prior to executing the import. The NetworkID setting can be found within the "applicationSettings" section of the config file.

3. Execute the import application by either double-clicking, or via command line. The console window will show progress messages of the import and any errors, the messages are also logged to the file "log.txt". When complete the console window will closed if launched by double-clicking, or indicate that it is finished if exectuted via command line.