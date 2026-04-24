# Pharmacy

> Production-ready система управления аптекой с микросервисной архитектурой

---

## Описание

**Pharmacy** — это серверная система управления аптекой, развёрнутая в Docker-контейнерах.  
Система включает REST API, реляционную базу данных, кэширование, мониторинг, CI/CD и клиент с делегатами и событиями.

---

## Основные возможности

- Полный CRUD для препаратов, поставщиков, рецептов и продаж
- Учёт остатков медикаментов на складе
- Привязка к пациенту, врачу и препарату
- Кэширование данных через Redis
- Мониторинг метрик в реальном времени через Grafana
- Автоматический CI/CD пайплайн через GitHub Actions

---

## Технологии

| Технология | Назначение |
| --- | --- |
| **C# / .NET 8** | Основной язык и платформа |
| **ASP.NET Core Web API** | REST API сервер |
| **Entity Framework Core 8** | Миграции базы данных |
| **PostgreSQL 15** | Реляционная база данных |
| **Redis 7** | Кэширование|
| **Nginx** | Обратный прокси |
| **Prometheus** | Сбор метрик |
| **Grafana** | Визуализация метрик |
| **Docker / Docker Compose** | Контейнеризация всех сервисов |
| **GitHub Actions** | CI/CD пайплайн |

---

## Архитектура

Система состоит из 6 Docker-контейнеров в единой сети:

```
Браузер / Клиент
    → Nginx (:8080)
        → API (ASP.NET Core)
            → PostgreSQL (данные)
            → Redis (кэш)

Prometheus → /metrics (API)
Grafana    → Prometheus
```

---

## Сущности предметной области

| Сущность | Описание |
| --- | --- |
| **Medicine** | Препарат: название, описание, цена, остаток, признак рецептурного |
| **Supplier** | Поставщик: наименование, телефон, email, адрес |
| **Sale** | Продажа: препарат, рецепт, количество, сумма, дата, покупатель |

---

## Как запустить

### Требования

- [Docker](https://docs.docker.com/get-docker/) с плагином Compose v2
- Git

### Запуск

1. Клонируйте репозиторий:

```bash
git clone https://github.com/danilaMeow/pharmacy.git
cd pharmacy
```

2. Запустите всю систему одной командой:

```bash
docker compose up --build -d
```

3. Откройте в браузере:

| Сервис | Адрес |
| --- | --- |
| Swagger UI | http://localhost:8080/swagger |
| Grafana | http://localhost:3000 (логин:admin/пароль:admin) |
| Prometheus | http://localhost:9090 |

### Остановка

```bash
docker compose down
```

### Сброс базы данных

```bash
docker compose down -v
docker compose up --build -d
```

---

## API Эндпоинты 

### Препараты

| Метод | Путь | Описание | Кэш |
| --- | --- | --- | --- |
| GET | /api/medicines | Список всех препаратов | Redis 5 мин |
| GET | /api/medicines/{id} | Препарат по ID | Redis 5 мин |
| GET | /api/medicines/search?name=... | Поиск по названию | — |
| POST | /api/medicines | Создать препарат | инвалидация |
| PUT | /api/medicines/{id} | Обновить препарат | инвалидация |
| DELETE | /api/medicines/{id} | Удалить препарат | инвалидация |

### Поставщики

| Метод | Путь | Описание | Кэш |
| --- | --- | --- | --- |
| GET | /api/suppliers | Список поставщиков | Redis 5 мин |
| GET | /api/suppliers/{id} | Поставщик по ID | Redis 5 мин |
| POST | /api/suppliers | Добавить поставщика | инвалидация |
| PUT | /api/suppliers/{id} | Обновить поставщика | инвалидация |
| DELETE | /api/suppliers/{id} | Удалить поставщика | инвалидация |

### Продажи

| Метод | Путь | Описание |
| --- | --- | --- |
| GET/POST/DELETE | /api/sales | CRUD продаж |
| GET | /api/sales/report | Отчёт по продажам |

---

## Клиентское приложение (Pharmacy.Client)

Консольный C#-клиент демонстрирует применение делегатов и событий:

- Собственный тип делегата `OnRequestCompleted(endpoint, statusCode, elapsedMs)`
- Многоадресный делегат: логирование в файл + вывод
- `Action<T>` и `Func<T, TResult>` для CRUD-операций
- Динамическая подписка/отписка через `+=` / `-=`

```bash
cd src/Pharmacy.Client
dotnet run
```

## CI/CD 

При каждом push в ветку `main` автоматически запускается пайплайн GitHub Actions:

1. Получение кода репозитория
2. Установка .NET SDK 8.0
3. Восстановление зависимостей (`dotnet restore`)
4. Сборка проекта (`dotnet build`)
5. Запуск unit-тестов (`dotnet test`)

---

## Автор

Митрофанов Д.К. [@danilaMeow](https://github.com/danilaMeow)
