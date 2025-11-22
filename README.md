## Техническое задание на backend для Ecommerce (Fastcart)

Данный документ описывает **требования к backend‑части интернет‑магазина (Ecommerce)** по мотивам дизайна Figma «Fastcart».  
Backend должен предоставлять **REST API** для Web/Mobile‑клиентов и быть реализован на стеке **.NET 9 + PostgreSQL + EF Core + Redis + Hangfire + Serilog** с использованием **Clean Architecture** и **Repository pattern**.

---

## 1. Цели, границы проекта и ожидаемый результат

### 1.1. Цели

- Разработать модульный и расширяемый backend, который:
  - обслуживает каталог товаров, баннеры, корзину, оформление заказов (checkout), личный кабинет и admin‑панель;
  - поддерживает авторизацию по JWT и разграничение прав (Customer / Admin / Manager);
  - обеспечивает необходимые нефункциональные требования (логирование, кеширование, фоновую обработку задач).

### 1.2. Роли пользователей

- **Customer (покупатель)**  
  - Регистрация, вход, просмотр каталога и баннеров, страница «О нас», управление корзиной и wishlist, оформление заказов, просмотр истории заказов, редактирование профиля.

- **Admin**  
  - Управление категориями, товарами, баннерами, статичным контентом (About, KPI, Team), заказами, пользователями и вручную задаваемыми курсами валют.

- **Manager (опционально)**  
  - Доступ к разделам по заказам и частично к клиентам без полного административного доступа.

### 1.3. Ожидаемый результат

- Готовое backend‑решение на .NET с:
  - архитектурой **Clean Architecture**;
  - слоем данных на **PostgreSQL** с миграциями EF Core;
  - реализованным **Repository pattern** и, при необходимости, **Unit of Work**;
  - документированным API (Swagger / OpenAPI);
  - интеграцией **Redis**, **Hangfire**, **Serilog**;
  - единой моделью ответа сервисов `ServiceResult<T> { StatusCode, Message, Data }`.

---

## 2. Стек технологий, архитектура и паттерны

### 2.1. Технологии

- **Платформа**: .NET **9.0.3**
- **Язык**: C#
- **Web‑фреймворк**: ASP.NET Core Web API
- **База данных**: PostgreSQL + драйвер Npgsql
- **ORM**: Entity Framework Core 9
- **Кеш**: Redis (StackExchange.Redis)
- **Фоновые задачи**: Hangfire
- **Логирование**: Serilog (Console + File, опционально Seq/PostgreSQL)
- **Маппинг моделей**: AutoMapper
- **Аутентификация и авторизация**: ASP.NET Core Identity + JWT Bearer
- **Валидация**: FluentValidation (предпочтительно) / DataAnnotations
- **Документация API**: Swagger

### 2.2. Архитектура (Clean Architecture)

Solution разделён на уровни:

- `Project.Domain`
  - Доменные сущности, value‑объекты, enum‑ы, доменные события (при необходимости).

- `Project.Application`
  - Интерфейсы сервисов и репозиториев, DTO/ViewModel, use case‑ы, обработчики команд и запросов, валидация.
  - Здесь сосредоточена бизнес‑логика.

- `Project.Infrastructure`
  - Реализация интерфейсов доступа к данным (репозитории), конфигурация DbContext (EF Core), адаптер Redis, интеграция с Hangfire, отправка почты, реализация Unit of Work.

- `Project.WebApi`
  - Controllers, endpoints, Middleware (обработка ошибок, логирование запросов), DI, конфигурация Serilog, Swagger, Hangfire Dashboard.

### 2.3. Repository pattern 
- `IProductRepository`, `ICategoryRepository`, `IOrderRepository`, `IBannerRepository` и т.д.

#### 2.3.1. ServiceResult

Все методы application‑сервисов возвращают:

- `Task<ServiceResult<T>>` или `Task<ServiceResult>`  
  где:
  - `StatusCode: int` – логический/HTTP код (`200`, `400`, `404`, `500`, и т.д.),
  - `Message: string` – человеко‑читаемое сообщение,
  - `Data: T` – полезная нагрузка (или `null` при ошибке).

Контроллеры WebApi возвращают объект с тем же форматом JSON, что упрощает обработку ошибок на фронтенде.

---

## 3. Доменные сущности

Ниже перечислены ключевые сущности домена. При необходимости их можно расширить полями аудита (`CreatedBy`, `UpdatedBy`) и т.п.

### 3.1. Пользователи и роли

**ApplicationUser** (наследник `IdentityUser<Guid>`):

- `Id: Guid`
- `Email: string` (уникальный, обязательный)
- `UserName: string`
- `FirstName: string`
- `LastName: string`
- `PhoneNumber: string?`
- `IsActive: bool`
- `CreatedAt: DateTime`
- `UpdatedAt: DateTime`
- `LastLoginAt: DateTime?`

**Роли (IdentityRole<Guid>):**

- `Customer`
- `Admin`
- `Manager` (опционально)

Изменение ролей возможно только через Admin API.

### 3.2. Category

- `Id: Guid`
- `Name: string`
- `Slug: string` (уникальный, для URL)
- `ParentCategoryId: Guid?` (иерархия категорий)
- `IsActive: bool`
- `SortOrder: int`
- `CreatedAt`, `UpdatedAt`

### 3.3. Product

- `Id: Guid`
- `Name: string`
- `Slug: string` (уникальный)
- `Description: string`
- `Price: decimal` – базовая цена (в основной валюте, например TJS)
- `Currency: string` – код валюты, например `"TJS"`
- `Size: string?` – один размер для товара (без вариаций)
- `Color: string?` – один цвет для товара
- `StockQuantity: int`
- `CategoryId: Guid`
- `IsActive: bool`
- `IsFeatured: bool` – использовать в блоке «Featured products»
- `CreatedAt`, `UpdatedAt`

### 3.4. ProductImage

- `Id: Guid`
- `ProductId: Guid`
- `ImageUrl: string`
- `IsMain: bool`
- `SortOrder: int`

### 3.5. Banner

- `Id: Guid`
- `Title: string?`
- `Subtitle: string?`
- `ImageUrl: string`
- `RedirectUrl: string?`
- `Position: string` – например: `HomeHero`, `HomeMiddle`, `HomeBottom`
- `SortOrder: int`
- `IsActive: bool`
- `StartDate: DateTime?`
- `EndDate: DateTime?`
- `CreatedAt`, `UpdatedAt`

### 3.6. Cart и CartItem

**Cart**

- `Id: Guid`
- `UserId: Guid` (на первой версии допускаем только авторизованных пользователей)
- `CreatedAt`, `UpdatedAt`

**CartItem**

- `Id: Guid`
- `CartId: Guid`
- `ProductId: Guid`
- `Quantity: int`
- `UnitPrice: decimal` – цена товара на момент добавления
- `CreatedAt`

### 3.7. Order и OrderItem

**Order**

- `Id: Guid`
- `OrderNumber: string` (человекочитаемый номер, например `ORD-2025-000123`)
- `UserId: Guid?`
- `Status: OrderStatus`  
  - `New`, `PendingPayment`, `Paid`, `Processing`, `Shipped`, `Completed`, `Canceled`
- Денежные поля:
  - `TotalAmountBase: decimal` – сумма в базовой валюте
  - `Currency: string` – валюта заказа (`USD`, `RUB`, …)
  - `CurrencyRate: decimal` – **курс, задаваемый вручную**, без интеграции с внешними API
- Адрес доставки:
  - `FullName`
  - `Phone`
  - `Country`
  - `City`
  - `AddressLine`
  - `PostalCode`
- Даты:
  - `CreatedAt`
  - `PaidAt?`, `ShippedAt?`, `CompletedAt?`, `CanceledAt?`

**OrderItem**

- `Id: Guid`
- `OrderId: Guid`
- `ProductId: Guid`
- `ProductNameSnapshot: string`
- `UnitPriceBase: decimal`
- `Quantity: int`
- `TotalBase: decimal`

### 3.8. Payment (опционально)

- `Id: Guid`
- `OrderId: Guid`
- `AmountBase: decimal`
- `PaymentMethod: string` (`CashOnDelivery`, `Card`, `Transfer`, …)
- `PaymentStatus: PaymentStatus` (`Pending`, `Paid`, `Failed`, `Refunded`)
- `TransactionId: string?`
- `CreatedAt`

### 3.9. Wishlist - Список желаний

- `Wishlist`
  - `Id: Guid`
  - `UserId: Guid`
  - `CreatedAt`
- `WishlistItem`
  - `Id: Guid`
  - `WishlistId: Guid`
  - `ProductId: Guid`
  - `CreatedAt`

### 3.10. Статический контент (About / Our Story)

- `AboutPage`
  - `Id: Guid`
  - `Title: string`
  - `Body: string` (HTML/Markdown)
  - `MainImageUrl: string`
  - `CreatedAt`, `UpdatedAt`
- `AboutKpi`
  - `Id: Guid`
  - `Icon: string`
  - `Title: string`
  - `Value: string` (10.5k, 33k, …)
  - `SortOrder: int`
  - `IsActive: bool`
- `TeamMember`
  - `Id: Guid`
  - `FullName: string`
  - `Position: string`
  - `PhotoUrl: string`
  - `FacebookUrl?`, `InstagramUrl?`, `LinkedInUrl?`
  - `SortOrder: int`
  - `IsActive: bool`

---

## 4. Redis и стратегия кеширования

### 4.1. Что кешируем

- **Категории** – `categories:all`
- **Списки товаров** – `products:list:{filter-hash}`
- **Баннеры** – `banners:{position}`
- **Конфигурации** – при необходимости (`config:*`)

### 4.2. Подход (Cache‑aside)

1. Сервис пытается получить данные из Redis по ключу.  
2. При отсутствии – читает из БД через репозиторий.  
3. Сохраняет данные в Redis с TTL.  
4. Возвращает данные клиенту.

### 4.3. TTL и инвалидация кеша

- Категории, баннеры: **5–15 минут**.
- Списки товаров: **1–5 минут**.
- При изменении данных через Admin API:
  - при изменении категории – очищаются ключи `categories:*`;
  - при изменении товара – `products:*`;
  - при изменении баннеров – `banners:*`.

### 4.4. Интерфейс кеш‑сервиса

- `ICacheService`:
  - `Task<T?> GetAsync<T>(string key)`
  - `Task SetAsync<T>(string key, T value, TimeSpan ttl)`
  - `Task RemoveAsync(string key)`


---

## 5. Фоновые задачи (Hangfire)

### 5.1. Основные типы задач

- **Уведомления по почте**:
  - письмо после регистрации (welcome / подтверждение);
  - письмо‑подтверждение заказа;
  - уведомления об изменении статуса заказа (Shipped, Completed, Canceled).

- **Техническое обслуживание**:
  - очистка заброшенных корзин (старше N дней);
  - очистка устаревших refresh‑токенов (если применяются);
  - архивирование/очистка устаревших логов (при необходимости).

- **Аналитика (опционально)**:
  - пересчёт агрегированных показателей продаж для админ‑дашборда.

### 5.2. Конфигурация Hangfire

- Хранение заданий: PostgreSQL или Redis.
- Hangfire Dashboard:
  - URL: `/hangfire`
  - доступ только для пользователей с ролью `Admin`.

### 5.3. Триггеры

- При создании заказа:
  - планируется job на отправку письма с подтверждением.
- При смене статуса заказа:
  - планируется job с уведомлением клиента.
- Периодические задачи:
  - `CleanAbandonedCarts` – 1 раз в сутки;
  - `CleanExpiredTokens` – 1 раз в сутки;
  - `RebuildSalesStatistics` – 1 раз в сутки / неделю.

---

## 6. Логирование и мониторинг (Serilog)

### 6.1. Базовая настройка

- Источники логов:
  - Console (dev);
  - файл (rolling file, по дням);


- Каждая запись лога должна содержать:
  - время (`Timestamp`);
  - уровень (`Information`, `Warning`, `Error`, `Fatal`);
  - сообщение;
  - `CorrelationId` / `RequestId`;
  - `UserId` (если пользователь аутентифицирован);
  - стек исключения (при ошибках).

### 6.2. Что логировать

- Все необработанные исключения.
- Начало и завершение HTTP‑запросов (route, статус, длительность).
- События аутентификации (успешный/неуспешный вход, отказ в доступе).
- Критические действия администратора:
  - создание/изменение/удаление товаров, категорий, баннеров, участников команды;
  - изменение статуса заказа и валютных параметров.

---

## 7. Дизайн API и основные endpoints

### 7.1. Общие правила

- Все маршруты начинаются с `api/v1`.
- Ответ контроллера обёрнут в `ServiceResult<T>`:

```json
{
  "statusCode": 200,
  "message": "Success",
  "data": { }
}
```

- Для ошибок:
  - `statusCode` – 400, 401, 403, 404, 409, 500 и т.д.;
  - `data` – `null`.

### 7.2. Auth & Account

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh-token` (опционально)
- `POST /api/v1/auth/logout` (опционально)

- `GET /api/v1/account/profile` (Authorize)
- `PUT /api/v1/account/profile` (Authorize)
- `PUT /api/v1/account/change-password` (Authorize)

### 7.3. Каталог (Categories & Products)

**Публичные:**

- `GET /api/v1/categories`
- `GET /api/v1/products`
  - query‑параметры: `page`, `pageSize`, `categoryId`, `search`, `sortBy`, `minPrice`, `maxPrice`, `onlyFeatured`.
- `GET /api/v1/products/{slug}`

**Admin:**

- `GET /api/v1/admin/categories`
- `POST /api/v1/admin/categories`
- `PUT /api/v1/admin/categories/{id}`
- `DELETE /api/v1/admin/categories/{id}`

- `GET /api/v1/admin/products`
- `POST /api/v1/admin/products`
- `PUT /api/v1/admin/products/{id}`
- `DELETE /api/v1/admin/products/{id}`
- `POST /api/v1/admin/products/{id}/images`
- `DELETE /api/v1/admin/products/{id}/images/{imageId}`

### 7.4. Баннеры

**Публичные:**

- `GET /api/v1/banners/{position}`  
  пример: `/api/v1/banners/HomeHero`

**Admin:**

- `GET /api/v1/admin/banners`
- `GET /api/v1/admin/banners/{id}`
- `POST /api/v1/admin/banners`
- `PUT /api/v1/admin/banners/{id}`
- `DELETE /api/v1/admin/banners/{id}`
- `PUT /api/v1/admin/banners/{id}/status`
- `PUT /api/v1/admin/banners/reorder`

### 7.5. Корзина и оформление заказа

- `GET /api/v1/cart`
- `POST /api/v1/cart/items`
- `PUT /api/v1/cart/items/{itemId}`
- `DELETE /api/v1/cart/items/{itemId}`
- `DELETE /api/v1/cart`

**Checkout / заказы (Customer):**

- `POST /api/v1/checkout`
  - Вход:
    - данные доставки;
    - валюта и курс (`Currency`, `CurrencyRate`), задаваемые вручную;
  - Выход:
    - информация о созданном заказе, номер заказа.

- `GET /api/v1/orders/my`
- `GET /api/v1/orders/{orderNumber}`
- `POST /api/v1/orders/{orderNumber}/cancel`

### 7.6. Заказы – Admin

- `GET /api/v1/admin/orders`
  - фильтры: `status`, `fromDate`, `toDate`, `orderNumber`, `userEmail`.
- `GET /api/v1/admin/orders/{orderNumber}`
- `PUT /api/v1/admin/orders/{orderNumber}/status`
- `PUT /api/v1/admin/orders/{orderNumber}/currency`

### 7.7. Wishlist

- `GET /api/v1/wishlist`
- `POST /api/v1/wishlist/{productId}`
- `DELETE /api/v1/wishlist/{productId}`

### 7.8. About / Our Story (CMS)

**Публичный:**

- `GET /api/v1/content/about`
  - возвращает:
    - `AboutPage`,
    - список `AboutKpi`,
    - список `TeamMember`.

**Admin:**

- `GET /api/v1/admin/content/about`
- `PUT /api/v1/admin/content/about`
- `GET /api/v1/admin/content/about/kpis`
- `POST /api/v1/admin/content/about/kpis`
- `PUT /api/v1/admin/content/about/kpis/{id}`
- `DELETE /api/v1/admin/content/about/kpis/{id}`

- `GET /api/v1/admin/content/about/team-members`
- `POST /api/v1/admin/content/about/team-members`
- `PUT /api/v1/admin/content/about/team-members/{id}`
- `DELETE /api/v1/admin/content/about/team-members/{id}`

---

## 8. Пользовательские сценарии

### 8.1. Сценарий Customer: от захода на сайт до оформления заказа

1. Пользователь открывает главную страницу:
   - фронт запрашивает `GET /api/v1/banners/HomeHero` и список популярных товаров;
   - данные берутся из Redis либо из БД через репозиторий.
2. Пользователь просматривает категории (`GET /api/v1/categories`) и списки товаров.
3. Открывает карточку товара (`GET /api/v1/products/{slug}`).
4. Регистрируется или входит:
   - `POST /api/v1/auth/register` или `POST /api/v1/auth/login`.
5. Добавляет товары в корзину (`POST /api/v1/cart/items`).
6. Смотрит корзину (`GET /api/v1/cart`).
7. Переходит к оформлению заказа:
   - вводит адрес и выбирает валюту/курс;
   - отправляет `POST /api/v1/checkout`;
   - backend создаёт заказ, очищает корзину, ставит email‑уведомление в очередь Hangfire.
8. Позже пользователь может просматривать свои заказы:
   - `GET /api/v1/orders/my`, `GET /api/v1/orders/{orderNumber}`.

### 8.2. Сценарий Admin: управление контентом и заказами

1. Admin входит через `POST /api/v1/auth/login` (роль `Admin`).
2. Управляет каталогом:
   - создаёт/редактирует/удаляет категории и товары;
   - при этом сервисы инвалидируют соответствующий кеш в Redis.
3. Управляет баннерами:
   - добавляет баннеры для разных позиций на главной странице;
   - задаёт период активности (StartDate/EndDate) и порядок сортировки.
4. Редактирует страницу «О нас»:
   - меняет основной текст, KPI и участников команды.
5. Работает с заказами:
   - фильтрует и просматривает заказы;
   - меняет статус (New → Paid → Shipped → Completed / Canceled);
   - при изменении статуса создаётся запись в логе и очередное задание на отправку email.

---

## 9. Нефункциональные требования

- **Безопасность**
  - JWT Bearer для всех защищённых endpoint‑ов;
  - разграничение прав по ролям;
  - обязательное использование HTTPS в production.

- **Производительность**
  - Redis‑кеш для часто читаемых данных;
  - пагинация при выдаче больших списков (товары, заказы, пользователи).

- **Надёжность**
  - Hangfire с политикой повторных попыток;
  - подробное логирование в Serilog.

- **Миграции БД**
  - управление схемой БД через EF Core Migrations;
  - подготовка скриптов для CI/CD.


---

## 10. Требования к качеству кода

- Соблюдение принципов Clean Architecture и SOLID.
- Вся бизнес‑логика в слое `Application` (сервисы, use case‑ы); контроллеры максимально тонкие.
- Использование `Repository pattern` .
- Все публичные методы сервисов:
  - асинхронные (`async/await`);
  - возвращают `ServiceResult<T>` / `ServiceResult`.
- Чёткое разделение доменных сущностей и DTO (модели для API); маппинг выполняется через AutoMapper.
- Организация unit‑тестов для ключевых сервисов (OrderService, ProductService и т.п.) по возможности.




