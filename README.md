# Currency Service

Микросервисная система для работы с курсами валют.

## Требования
- .NET 8 SDK

## Архитектура
- Clean Architecture + CQRS (MediatR) 
- YARP Gateway 
- JWT Auth 
- EF Core + PostgreSQL 
- Background Worker (CBR Parser)

## Тестирование
- Файл test-api.http содержит готовые запросы для Rider HTTP Client.

## Быстрый старт

```bash
# 1. Клонировать репозиторий
gh repo clone hagakoure/CurrencyService
cd CurrencyService

# 2. Запустить через Docker
docker-compose up --build -d

# 3. Дождаться запуска и открыть:
#    - API Gateway: http://localhost:5000
#    - Swagger (в режиме Development): http://localhost:5000/swagger