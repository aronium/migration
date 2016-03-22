# Aronium Migration

Aronium.Migration is a console application written in C# used for managing migrations for SQL Server and SQLite databases.

It uses similar commands as [MyBatis Migrations](http://www.mybatis.org/migrations/)

 * `bootstrap` - Initializes database configuration
 * `status` - Gets the migrations status
 * `new` - Creates new migration script
 * `up` - Executes migration scripts 
 * `help` - Displays help

## Usage

#### Bootstrap
Navigate to directory containing `migration.exe` and execute the following command:
```
> migration bootstrap
```
**SQL Server**

Executing `bootstrap` command will ask for a server and credentials to use
Follow the instructions to define connection parameters like server name, database and user credentials.

**SQLite**

Executing `bootstrap` command will ask for a database file (SQLite database)

> Database file:

Write file name or a full path to your SQLite database
> Selected file: C:\...\demo.sqlite
> 
> Specified file do not exists. Do you want to create new database file? Y/N

If you select `Y` SQLite database will be automatically created on specified location.

#### New
Navigate to directory containing `migration.exe` and execute the following command:
```
> migration -eew "My First Migration Script"
```
This command will output the result as:
> New script added. Path: C:\...\Migrations\Scripts\1_0__My_First_Migration_Script.sql

Running "new" command without specified version name will add new file with major version automatically generated.
