# MongoDB_Empty_Pattern

## Описание

Проект MongoDB_Empty_Pattern предоставляет API для управления лицензиями Arca(сойдёт и для любого другого проекта) и пользователями с использованием базы данных MongoDB. API включает функционал создания, активации, деактивации, удаления и продления лицензий, а также регистрацию и авторизацию пользователей.

## Установка и настройка

1. Склонируйте репозиторий:
   ```bash
   git clone https://github.com/GOSUKZ/MongoDB_Empty_Pattern.git
   ```

2. Перейдите в директорию проекта:
   ```bash
   cd MongoDB_Empty_Pattern
   ```

3. Установите зависимости:
   ```bash
   dotnet restore
   ```

4. Настройте параметры подключения к базе данных и JWT в файле `appsettings.json`.

5. Запустите проект:
   ```bash
   dotnet run
   ```

## Контроллеры

### LicensesController

#### Привязка MAC-адреса к лицензии
```http
POST /licenses/bindMacAddress
```
Параметры:
- `licenseKey` (string, обязательный)
- `macAddress` (string, обязательный)

#### Создание лицензии
```http
POST /licenses/createLicense
```
Параметры:
- `org` (string, обязательный)
- `expire_date` (DateTime, обязательный)
- `BIN` (string, обязательный)

#### Активация лицензии
```http
POST /licenses/activateLicense/{License_Code}
```
Параметры:
- `License_Code` (string, обязательный)

#### Деактивация лицензии
```http
POST /licenses/deactivateLicense/{License_Code}
```
Параметры:
- `License_Code` (string, обязательный)

#### Удаление лицензии
```http
DELETE /licenses/deleteLicense/{License_Code}
```
Параметры:
- `License_Code` (string, обязательный)

#### Продление лицензии
```http
POST /licenses/extendLicense/{License_Code}
```
Параметры:
- `License_Code` (string, обязательный)
- `newExpireDate` (DateTime, обязательный)

#### Получение одной лицензии
```http
GET /licenses/getLicense/{License_Code}
```
Параметры:
- `License_Code` (string, обязательный)

#### Получение всех лицензий
```http
GET /licenses/getAllLicenses
```

### UsersController

#### Логин пользователя
```http
POST /api/users/Login
```
Параметры:
- `username` (string, обязательный)
- `password` (string, обязательный)

#### Регистрация пользователя
```http
POST /api/users/Registration
```
Параметры:
- `username` (string, обязательный)
- `password` (string, обязательный)

## Авторы

- Серкали Динмухамед
- https://github.com/DeStiNoVich21
