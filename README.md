DeployScript
============

Facilitate deploying web sites to different Windows (IIS/MsSQL) servers by scripting.


You define a set of ".deploy" scripts (probably one for each server) and upload alongside with your website files. 
Then DeployScript runs those scripts and changes configurations, databases, etc. based on the server. 
This way you will never forget to apply a certain changes after deploying to a specific server. 
An example is when something must be ON in test server but OFF for production server.

A deploy script looks like an .ini file:

```ini
# any line started with # is a comment and will be ignored
# variables:
# %server% can be test.ge , uat.co, etc.
# (hint: for server name comes from C:\servername.txt or env var MACHINE_ALIAS or COMPUTERNAME
# %db% it’s database name extracted from connection string inside web.config
# %path_web% full path to Website folder
# %path_webconfig%
# %connection_string%
#
# deploy_script.exe reads a file named start.deploy and runs its contents
# then based on the server name runs machine specific file (%server%.deploy ,if exists)
# then it tries to find others.deploy and run it

#at any point you can include another script
#included script can use defined variables here and defined variables will become visible to this
include common.deploy

[define]
some_var=value
based_on_thers= %some_var%123
escaped=%this is an escape 100\% this one %%two%

connection_string = abc
# this undefines a variable (for system  it means return to default)
connection_string = 


# this section updates database using model
[database]

# translates to UPDATE Users SET Password=’123’ WHERE name=’admin@uat.co’
Users[name='admin@uat.co'].Password = '123'
Users[name<>'admin@uat.co'].Password = 'test'
Users[created>='2012-1-2'].Password = null

# if filter is on "Name" column you can omit name= part
Settings['current'].WebsiteName = '%server%'

Clients[id=’g-u-i-d’].Something = 10

# this runs a whole sql file which is inside the current path
Exec sql_file_name.sql
Exec server_specific_%server%.sql

# also at any point you can use print command for debug or other purposes
print data base is %db%

# this section updates web.config
[settings]
ftp.server = 10.0.0.1
ftp.user = john
something = 'a=%db%' this is \%escaped\%
x[a] = b
x[a=b] = c
```
