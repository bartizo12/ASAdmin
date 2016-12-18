AS.Infrastructure
-----------------
Core Infrastructure of the project. This project covers  Data Access,ORM , Logging, IOC, Scheduling,
Membership (Microsoft.Identity.Framework) and other 3rd part integrations.
It is important to keep this project platform independent. That is why this project should not have
any web dependency. 
It will be also good example if we make our ORM/data access db provider agnostic, so that 
it can work with any db provider. For now, it only supports  Sql Server,Sql Server CE and MySql.


Dependencies
-------------
AS.Domain.Entities
AS.Domain.Interfaces
AS.Domain.Parameters

Third Party Dependencies
------------------------
Autofac
EntityFramework
EntityFramework.SqlServer
EntityFramework.SqlServerCompact
FluentScheduler
Microsoft.AspNet.Identity.Core
Microsoft.AspNet.Identity.EntityFramework
MySql.Data
MySql.Data.Entity.EF6
Newtonsoft.Json
NLog



