# OnlineStoreMonitoring

REST API для интернет-магазина с системой мониторинга активности пользователей. Разработан в рамках производственной практики по специальности 09.02.07 «Информационные системы и программирование».

**Автор:** Абрамов Лев Расулович
**Группа:** ДКИП-304прог
**Место практики:** ООО «СИМУЛТЕХ»
**Период:** 01.06.2026 – 28.06.2026

---

## О проекте

Backend-приложение фиксирует действия покупателей в интернет-магазине, формирует аналитику по конверсии и популярности товаров, выгружает отчёты в CSV. Реализовано на ASP.NET Core 8 с многослойной архитектурой.

## Стек технологий

- **Платформа:** .NET 8, ASP.NET Core Web API
- **Язык:** C# 12
- **База данных:** SQL Server LocalDB
- **ORM:** Entity Framework Core 8.0
- **Аутентификация:** JWT Bearer
- **Документация API:** Swagger UI (Swashbuckle)
- **Кэширование:** IMemoryCache
- **IDE:** JetBrains Rider

## Архитектура

Проект построен по принципу многослойной архитектуры:

```
OnlineStoreMonitoring/
├── Domain/Entities/           // Доменные сущности
│   ├── User.cs
│   ├── Product.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   └── CustomerEvent.cs
├── Application/
│   ├── DTOs/                  // Объекты передачи данных
│   ├── Repositories/          // Repository Pattern
│   └── Services/              // Бизнес-логика
├── Infrastructure/
│   ├── Data/                  // DbContext
│   └── Logging/               // Сервис логирования
├── Presentation/Controllers/  // REST API контроллеры
├── Program.cs                 // Точка входа
└── appsettings.json           // Конфигурация
```

## Возможности

- Регистрация и аутентификация через JWT с разделением ролей Admin и Customer
- CRUD-операции с товарами, заказами и событиями
- Запись трёх типов событий: ProductView, CartAdd, Order
- Аналитика конверсии воронки продаж
- Статистика по статусам заказов
- Топ популярных товаров
- Генератор тестовых данных
- Логирование операций
- Кэширование результатов аналитики на 10 минут
- Выгрузка CSV-отчётов

## Быстрый старт

### Предварительные требования

- .NET 8 SDK
- SQL Server LocalDB (входит в Visual Studio / Rider)
- JetBrains Rider или Visual Studio 2022

### Установка

```bash
git clone https://github.com/USERNAME/OnlineStoreMonitoring.git
cd OnlineStoreMonitoring
dotnet restore
```

### Применение миграций

В Package Manager Console внутри Rider:

```
Add-Migration InitialCreate
Update-Database
```

### Запуск

```bash
dotnet run
```

Swagger UI откроется по адресу: `http://localhost:5184/swagger`

## REST API

### Аутентификация

| Метод | Endpoint | Описание |
|-------|----------|----------|
| POST | `/api/Auth/register` | Регистрация пользователя |
| POST | `/api/Auth/login` | Вход и получение JWT-токена |

### Товары

| Метод | Endpoint | Роль | Описание |
|-------|----------|------|----------|
| GET | `/api/Product` | публичный | Список всех товаров |
| GET | `/api/Product/{id}` | публичный | Товар по ID |
| GET | `/api/Product/category/{category}` | публичный | Товары по категории |
| POST | `/api/Product` | Admin | Создание товара |
| PUT | `/api/Product/{id}` | Admin | Обновление товара |
| DELETE | `/api/Product/{id}` | Admin | Удаление товара |

### Заказы

| Метод | Endpoint | Роль | Описание |
|-------|----------|------|----------|
| POST | `/api/Order` | Customer/Admin | Создание заказа |
| GET | `/api/Order/{id}` | Customer/Admin | Заказ по ID |
| GET | `/api/Order/user/{userId}` | Customer/Admin | Заказы пользователя |
| GET | `/api/Order/status/{status}` | Admin | Заказы по статусу |
| PUT | `/api/Order/{id}/status` | Admin | Изменение статуса |

### События

| Метод | Endpoint | Роль | Описание |
|-------|----------|------|----------|
| POST | `/api/Event/track` | Customer/Admin | Запись события |
| GET | `/api/Event/user/{userId}` | Customer/Admin | События пользователя |
| GET | `/api/Event/product/{productId}` | публичный | События по товару |
| GET | `/api/Event/type/{eventType}` | публичный | События по типу |

### Аналитика

| Метод | Endpoint | Роль | Описание |
|-------|----------|------|----------|
| GET | `/api/Analytics/conversion` | Admin | Конверсия воронки продаж |
| GET | `/api/Analytics/order-status` | Admin | Распределение по статусам |
| GET | `/api/Analytics/popular-products` | Admin | Топ популярных товаров |

### Отчёты и генератор

| Метод | Endpoint | Роль | Описание |
|-------|----------|------|----------|
| POST | `/api/DataGenerator/generate` | Admin | Генерация тестовых данных |
| GET | `/api/Report/events-csv` | Admin | CSV-отчёт по событиям |
| GET | `/api/Report/orders-csv` | Admin | CSV-отчёт по заказам |
| GET | `/api/Report/analytics-csv` | Admin | Сводная аналитика в CSV |

## Пример использования

### 1. Регистрация администратора

```http
POST http://localhost:5184/api/Auth/register
Content-Type: application/json

{
  "username": "admin",
  "email": "admin@store.ru",
  "password": "Admin12345",
  "role": "Admin"
}
```

### 2. Получение JWT-токена

```http
POST http://localhost:5184/api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin12345"
}
```

### 3. Запись события просмотра товара

```http
POST http://localhost:5184/api/Event/track
Authorization: Bearer <ваш_токен>
Content-Type: application/json

{
  "userId": 1,
  "productId": 65,
  "eventType": "ProductView",
  "details": "Просмотр карточки товара"
}
```

### 4. Генерация тестовых данных

```http
POST http://localhost:5184/api/DataGenerator/generate?userCount=50&productCount=100&eventCount=999
Authorization: Bearer <ваш_токен>
```

## Структура базы данных

| Таблица | Описание |
|---------|----------|
| Users | Учётные записи покупателей и администраторов |
| Products | Каталог товаров с категориями |
| Orders | Заказы покупателей с статусами |
| OrderItems | Позиции в составе заказа |
| CustomerEvents | События активности покупателей |

## Типы событий

- `ProductView` — просмотр товара
- `CartAdd` — добавление в корзину
- `Order` — оформление заказа

## Статусы заказов

`Pending` → `Confirmed` → `Shipped` → `Delivered` (или `Cancelled`)

## Конфигурация

Файл `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OnlineStoreMonitoringDb;Trusted_Connection=true;"
  },
  "Jwt": {
    "SecretKey": "your-very-long-secret-key-for-jwt-token-generation-at-least-32-characters",
    "Issuer": "OnlineStoreMonitoring",
    "Audience": "OnlineStoreMonitoringApp",
    "ExpirationMinutes": 1440
  }
}
```

## Тестирование

Все эндпоинты тестируются через Swagger UI. После запуска проекта откройте `http://localhost:5184/swagger`, авторизуйтесь через кнопку «Authorize» (введите `Bearer <ваш_токен>`) и проверяйте функциональность.

## Результаты работы

После генерации тестовых данных получены следующие аналитические показатели:

- Всего просмотров товаров: 355
- Добавлений в корзину: 306
- Оформлено заказов: 338
- Коэффициент конверсии: 95,21%

Топ-3 популярных товаров: Laptop #65, Headphones #67, Smartphone #92.

## Лицензия

Проект разработан в учебных целях в рамках производственной практики.

## Автор

**Абрамов Лев Расулович**
Группа ДКИП-304прог
Специальность 09.02.07 «Информационные системы и программирование»
2026 год
