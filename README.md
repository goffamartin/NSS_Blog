# NSS Blog

Tento projekt je ukázková blogovací aplikace postavená na .NET Aspire. Obsahuje více služeb a demonstruje použití databáze, cache, messagingu, zabezpečení i full‑textového vyhledávání.

## Struktura repozitáře

- **Blog.ApiService** – hlavní REST API s Entity Frameworkem, Redis cache a RabbitMQ.
- **Blog.ElasticService** – služba nad Elasticsearch pro vyhledávání.
- **Blog.ElasticWorker** – background worker synchronizující články do Elasticsearch.
- **Blog.Web** – Blazor serverové UI volající API.
- **Blog.AppHost** – orchestrace všech služeb přes Docker Compose.
- **Blog.Tests** – integrační a unit testy.
- **Shared** – sdílené DTO objekty.

## Architektura

Systém je navržen jako **event‑based** mikroslužba se samostatnými službami pro API, vyhledávání a asynchronní zpracování. Hlavní API obsluhuje CRUD operace a současně publikuje události do RabbitMQ, čímž odděluje změny dat od jejich dalšího zpracování. Worker služba tyto události odebírá a indexuje články do Elasticsearch. K rychlému čtení slouží Redis cache. Celá infrastruktura je spravována pomocí Docker Compose / Aspire, což usnadňuje lokální i produkční nasazení. Frontendová část (Blazor Server) komunikuje s API přes generované HTTP klienty a využívá tokenovou autentizaci.

### Využité technologie

- **SQL Server** – relační databáze (soubor `BlogDbContext.cs`).
- **Redis** – distribuovaná cache (`RedisCacheService`).
- **RabbitMQ** – messaging (`RabbitPublisher`, `ArticleSyncWorker`).
- **Elasticsearch** – full‑textové vyhledávání (`Blog.ElasticService`).
- **ASP.NET Identity** – zabezpečení s tokeny (OAuth2).
- **RequestLoggingMiddleware** – interceptor logující každý request.

## Design patterns

1. **Dependency Injection** – všechny služby jsou registrovány a injektovány přes vestavěný kontejner.
2. **Decorator** – cache vrstva obaluje služby (`CachedArticleService`, `Program.cs`).
3. **Factory Method** – `InMemoryDbContextFactory` vytváří testovací databázi.
4. **Observer / Publish‑Subscribe** – RabbitMQ události a worker (`ArticleService` ↔ `ArticleSyncWorker`).
5. **Data Transfer Object** – všechny DTO třídy ve složce `Shared/Dtos`.
6. **Chain of Responsibility** – middleware pipeline včetně `RequestLoggingMiddleware`.

## Umístění hlavní funkcionality

- **Databáze a migrace** – `Blog.ApiService/Data` a `Migrations`.
- **Seed dat** – třídy `DataSeeder` a `IdentitySeeder` plus endpoint `/seed` v `Program.cs`【F:Blog.ApiService/Program.cs†L91-L99】.
- **Cache** – implementace v `Blog.ApiService/Cache` a registrace v `Program.cs`【F:Blog.ApiService/Program.cs†L53-L68】.
- **Messaging** – publikování událostí v `ArticleService`【F:Blog.ApiService/Services/ArticleService.cs†L41-L74】 a zpracování ve `ArticleSyncWorker`【F:Blog.ElasticWorker/ArticleSyncWorker.cs†L144-L195】.
- **Zabezpečení** – konfigurace Identity a bearer tokenu v `Program.cs`【F:Blog.ApiService/Program.cs†L43-L51】.
- **Interceptors** – `RequestLoggingMiddleware` použitý v `Program.cs`【F:Blog.ApiService/Program.cs†L120-L122】.
- **Elasticsearch klient** – registrace v `Blog.ElasticService/Program.cs`【F:Blog.ElasticService/Program.cs†L7-L9】.

## Use cases

1. **Správa uživatelů** – registrace, přihlášení a správa profilu přes ASP.NET Identity.
2. **Správa článků** – CRUD operace nad články, komentáře a lajky přes REST API (`ArticlesController`, `CommentsController`, `LikeController`).
3. **Vyhledávání** – uživatel zadá dotaz a API vrátí výsledky z Elasticsearch (`/search/articles`).

## Inicializační postup

1. Nainstalujte **.NET 9** SDK a příkaz `dotnet workload install aspire --skip-manifest-update --version 9.3.1`.
2. Otevřete řešení `Blog.sln` ve Visual Studiu 2022 nebo spusťte z příkazové řádky:
   ```bash
   dotnet restore
   dotnet aspire run
   ```
   Tím se spustí všechny služby definované v `Blog.AppHost`.
3. Alternativně lze použít připravený Docker Compose:
   ```bash
   docker compose up -d
   ```
4. Po spuštění (v dev prostředí) lze databázi naplnit testovacími daty:
   ```bash
   curl http://localhost:8011/seed
   ```
5. Frontend běží na adrese `http://localhost:8015`.

Migrace databáze se spouští automaticky při startu API služby (`Program.cs`)【F:Blog.ApiService/Program.cs†L113-L118】.

## Spuštění testů

Testy lze spustit příkazem:
```bash
dotnet test
```

## Nasazení

Projekt je připraven na běh v Docker Compose (`docker-compose.yaml`). Nasazení na veřejný server (například Heroku) je možné po úpravě kontejnerů.