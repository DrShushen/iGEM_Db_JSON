## Synopsis

A tool to convert iGEM Repository SQL Data to AlphaBrick's JSON format. iGEM SQL data is stored in the igem_simplified SQL database, derived from iGEM's own SQL dump.

## Requirements

* .NET Framework 4.5 or later
* MySQL 5.7.13 or later
* The igem_simplified MySQL schema running on localhost.
* MySQL Connector/Net 6.8.3 or later

## Installation and Usage

1. If you don't have igem_simplified MySQL schema running on localhost, restore one from the SQL dump supplied with project.
2. Modify the connectionString user and password in App.config to use your user and password.
3. Ensure that the following location exists: C:\Temp\ folder (JSON file is output here).
4. Execute the iGEM_Db_JSON.exe.

## License

Undetermined.