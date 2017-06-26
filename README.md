# ASPWiki [![Build Status](https://travis-ci.org/Kasperki/ASPWiki.svg?branch=master)](https://travis-ci.org/Kasperki/ASPWiki)

Very simple wiki made with ASP Net core as a training project. 

# Documentation



## Requirements

* dotnet core 1.0
* Mongodb 
* TwitterApp for authentication

### Setup mongodb user for test

Mongo 2.8
```
mongod --run mongo
mongo --open mongo client

//Local test db
use wiki
db.createUser({user: "DATABASEUSER", pwd: "PASSWORD", roles: [ { role: "readWrite", db: "wiki" }]})

//Unit tests db
use wikipagesTest
db.createUser({user: "root", pwd: "root", roles: [ { role: "readWrite", db: "wikipagesTest" }]})
```

## Installation
 Setup Environment variables
```
ASPNETCORE_ENVIRONMENT = "Development | Production"
ASPWiki:TwitterKey = "TwitterApiKey for authentication with twitter",
ASPWiki:TwitterKeySecret = "TwitterApiKeySecret for authentication with twitter",
ASPWiki:DatabaseUser = "Mongodb database user name",
ASPWiki:DatabasePassword = "Mongodb database user password",
```
Install packages
```
dotnet install
```
Run application
```
dotnet run
```

# Test
Run with:
```
dotnet test
```

# TODO

* Features
    * Commeting to spesific line of content
* Refactor
    * Parent/Path 
        * Wikipage Guid Parent;
        * Wikipage List<Guid> Childpages;
            *  Path = etc... parent.parent.Title + "/" + parent.Title + "/" + title;
        * When changing parent check that the page is not in the parent tree.
* Imporovements
    * Whitelisted user repository 
        * AuthenticationService would query whitelisted users from there
        * Add users command
        * Remove users command
    * SOC (Separation of concerns)
        * DataTransfer objects to own solution (objects that live in database)
    * DRY (Don't repeat yourself)
        * JqueryValidation for forms, add tags to models, don't repeat in views.