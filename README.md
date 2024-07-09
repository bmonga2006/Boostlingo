# Boostlingo
Boostlingo - Take home assignment

Assumptions : I assumed ID(from json file)  is unique for each person

Setup Instructions

Database Setup:
Use SQL Server to create the database and tables by executing the script provided in the SQL folder.

Configuration:
Open the Visual Studio solution and navigate to the appsettings.json file.
In the DefaultConnection key, replace the Data Source with your server name and the Initial Catalog with your database name.

Logging Configuration:
In the SeriLog section of the appsettings.json file, update the file path to an absolute path on your computer.

Run the Application:
Start the application.

Further Optimizations
Further optimizations could be achieved by creating indexes after data insertion. However, since this was a small dataset, I did not implement these optimizations. This approach would be beneficial when handling larger datasets, but we may still have to negotiate performance overheads depending on the nature of that application.

Unit Test
The tests in this application are sample tests and do not guarantee maximum coverage.


