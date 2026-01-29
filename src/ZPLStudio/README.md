# ZPL Studio (WPF)

## Настройка подключения

Строка подключения хранится в user-secrets:

```bash
dotnet user-secrets set "ConnectionStrings:Oracle" "User Id=...;Password=...;Data Source=..."
```

`appsettings.json` оставлен без секретов намеренно.

## Пример scaffolding (EF Core 6 + Oracle)

```bash
dotnet ef dbcontext scaffold \
  "User Id=...;Password=...;Data Source=..." \
  Oracle.EntityFrameworkCore \
  --schema MLSOFT \
  --table LIST_FOR_TEKARTON_V \
  --context AppDbContext \
  --context-dir Data \
  --output-dir Data/Entities \
  --no-onconfiguring
```

После scaffolding проверьте, что сущность `ListForTekartonView` остаётся `HasNoKey()` и `ToView()`.
