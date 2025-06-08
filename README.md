# GuessWho - Игра "Угадай кто?"

## Обновление пакетов
```
dotnet tool update --global dotnet-ef
```
## Создание миграции
```
dotnet ef migrations add InitialCreate --context ApplicationDbContext --project Migrations
```

## Применение миграций
```
dotnet ef database update --context ApplicationDbContext --project Migrations
```