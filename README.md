# QuizMaker

## Docker
Position to root directory (**QuizMaker**) and run command:
```bash
docker compose up -d
```
Once started navigate to _swagger_ documentation [http://localhost:5008/swagger/index.html](http://localhost:5008/swagger/index.html).

## Local
__Prerequisite__: running database container on port 5433

Position to WebAPI directory (**QuizMaker/QuizMaker.API**) and run command:
```bash
dotnet run
```
Once started navigate to _swagger_ documentation [http://localhost:5158/swagger/index.html](http://localhost:5158/swagger/index.html).

## Tests
__Prerequisite__: running database container on port 5433

Position to test directory (**QuizMaker/QuizMaker.Tests**) and run command:
```bash
dotnet test
```


## Database migration
### Add a new database migration
Use following command from root repository directory (**QuizMaker**):
```bash
## (Windows)
dotnet ef migrations add <MigrationTitle> --project .\QuizMaker.Database\ --startup-project .\QuizMaker.API\
```

```bash
## (MacOS/Linux)
dotnet ef migrations add <MigrationTitle> --project ./QuizMaker.Database/ --startup-project ./QuizMaker.API/
```

### Update database
__Note__: migrations and database updates are set to run at the application start automatically

Use following command from root repository directory (**QuizMaker**):
```bash
## (Windows)
dotnet ef database update --project .\QuizMaker.Database\ --startup-project .\QuizMaker.API\
```

```bash
## (MacOS/Linux)
export ASPNETCORE_ENVIRONMENT='Local' && dotnet ef database update --project ./QuizMaker.Database/ --startup-project ./QuizMaker.API/
```

