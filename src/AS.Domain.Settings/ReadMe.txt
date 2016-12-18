AS.Domain.Settings
---------------------
This project contains setting class definitions. Settings are stored in an external  data source(in our application it is database). 
They are fetched from external data source and stored in memmory cache at runtime when the application is started.
It works similar to storing configuration/settings in .config file . However ,our way of settings/configuration management has following benefits ;

** Changing settings at run time dynamically while application is running
** Keeping config files clean and simple
** Easy to integrate and manage the application

Dependencies
------------
AS.Domain.Entities
AS.Domain.Interfaces
