# Aronium Migration

Aronium.Migration is a console application written in C# used for managing migrations for SQL Server and SQLite databases.

It uses similar commands as [MyBatis Migrations](http://www.mybatis.org/migrations/)

 * `bootstrap` - Initializes database configuration
 * `status` - Gets the migrations status
 * `new` - Creates new migration script
 * `up` - Executes new migration scripts 
 * `pending` - Runs all non executed migration scripts
 * `help` - Displays help

Commands without additional parameters can be executed with command name only, while commands that contains parameters must be executed with leading minus sign (-).

**IMPORTANT:** *bootstrap* command must be executed before other commands so database credentials and properties are set properly.

Note that *Aronium.Migration.SQLite* uses shared files from *Aronium.Migration* project, added as linked files.

## Usage

Before executing any command from console, navigate to directory containing `migration.exe` or specifiy its full path. 

#### Bootstrap command
```
> migration bootstrap
```
*SQL Server*

Executing `bootstrap` command will ask for a server and credentials to use
Follow the instructions to define connection parameters like server name, database and user credentials.

*SQLite*

Executing `bootstrap` command will ask for a database file (SQLite database)

> Database file:

Write file name or a full path to your SQLite database
> Selected file: C:\\...\demo.sqlite
> 
> Specified file do not exists. Do you want to create new database file? Y/N

If you press `Y`, SQLite database will be automatically created on specified location.

#### New command
```
> migration -new "My First Migration Script"
```
This command will output the result as:
> New script added. Path: C:\\...\Migrations\Scripts\1_0__My_First_Migration_Script.sql

**Make sure you execute "new" command with leading minus sign before command name, eg. *-new "script file name"*, so file name is parsed correctly**

If leading minus sign is ommited, you will be asked to enter file name again.

Running "new" command without specified version name will add new file with major version automatically generated.

#### Status command
```
> migration status
```
This command will output current migration status with executed and pending migration scripts.

#### Up command
```
> migration up
```
Running `up` command will apply newer migrations then the current version.


#### Pending command
```
> migration pending
```
Running `pending` command will execute all pending migrations.
