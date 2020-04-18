Database Compare

Overview:
Compare SQL Server database schema or data to generate a SQL script to bring a Target database in sync with a Source database.

Requirements:
The program requires the .Net Framework 4.7.2 or later. 
The SQL Server scripts generated will run on SQL Server 2016 or later. 
Windows authentication is used. The user must have permission to view the objects and data that will be compared.
The source code references the project in the DatabaseCommon repository.

Repository Contents:
Bin: The compiled program
Source: Visual Studio solution with the C# source code
SQLScripts: SQL Server scripts to set up test databases. 
	SourceDatabase.sql: Sets up a test Source database.
	TargetDatabase.sql: Sets up a test Target database.
	DropAllUserObjects.sql: Removes all user objects from a database - May be helpful to get a Terget database back to the starting state.
	DataDiffScript.sql: The expected output from running a Data comparison.
	SchemaDiffScript.sql: The expected output from running a Schema comparison.
	
Program Parameters:
Source Server: The SQL Server instance name for the Source database.
Source Database: The name of the Source database.	
Target Server: The SQL Server instance name for the Target database.
Target Database: The name of the Target database.	
Job Type: The comparison to run.
	Schema Compare: Compare the database objects (Tables, Views, Stored Procedures, etc). Create SQL to bring the Target database to match the Source database.
	Data Compare: Compare data in selected tables to bring the data in the Target database tables in sync with the Source database tables. The tables in the Source database that have an extended property IsReferenceTable with a value of TRUE will be the tables selected to compare.
The DatabaseCompare.exe.config configuration file will store default values for the Server and Database names.