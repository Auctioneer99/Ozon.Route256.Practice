## Домашнее задание седьмой недели обучения

Домашнее задание досточно творческое. Оон заключается в рефакторинге вашего сервиса и приведении его к стандартам чистого кода и чистой аритектуры.

1. Создать проект Domain, в него поместить доменную модель и вынести в нее всю необходимую бизнес-логику
2. Создать проект Infrastructure и перенести в него весь инфраструктурный код: работы с БД шардированием, ServiceDiscovery, Kafka и пр.
3. Сделать все классы в проекте Infrastructure с доступом internal.
4. Создать проект Application и реалзиовать в нем необходимые сервисы приложения
5. "Очистить" проект Host (текущий проект по-умолчанию), в нем должна остаться только логика gPRC сервиса, чтение из Кафки (если необходимо), маппинг.
6. *Примечание*: использовать MediatR допустимо, но не обязательно.

##### Задание со звездочкой
Создать классы для примитивных типов там где нужно и поместить в них необходимкю бизнес логику. Например, вместо `string Email { get; }` сделать класс `Email`/

Дедлайн: 4 ноября, 23:59 (сдача) / 7 ноября, 23:59 (проверка)

## Домашнее задание шестой недели обучения

1. Поднять в окружении 2 инстанса БД для order-service
2. Сконфигурировать service-discovery для новых инстансов (orders-1, buckets 0-3; orders-2, buckets 4-7)
3. Выбрать ключ(и) шардирования и реализовать функцию для определения бакета по ключу
4. Настроить connectionFactory на работу с шардированной базой через service discovery
5. Переделать мигратор на работу с шардированной базой
6. Выполнить миграции на шардированный кластер (по бакетам)
7. Перевести работу сервиса на шардированную базу

##### Задание со звездочкой
Реализовать индексный поиск для получения заказов не по ключу шардирования (согласно контрактам) (*).

### FAQ

##### Как быть с ручками, которые ищут не по ключу с пагинацией и т.п.?
Для выполнения задания достаточно, чтобы вы перебирали все заказы из всех бакетов.
Но для ендпоинтов, осущетсвляющих поиск не по ключу шарирования, можно реализовать глобальный индекс (ради алмазика).
Подсказка - в индекс можно добавить необходимые поля для фильтрации

##### Как быть с другими сущностями в нашей БД — склады/регионы?
Так как это справочники, то их состав допустимо просто копировать во все шарды и держать в каждом шарде копию.
Вы можете шардировать также и их, но не забудьте снять foreign key constraint, если они у вас были.

##### Как быть с транзакционностью?
Для выполнения задания с алмазиком при реализации глобального индекса НЕ требуется, чтобы его обновление было выполнено транзационно с
вставкой/изменением основной сущности, т.к. в общем случае оно будет выполняться на другом шарде и потребует распределенной транзакции.
При этом, следует сохранить транзакционность сохранения самих сущностей в рамках одного запроса.

##### Как быть с множественной вставкой?
Реализация вставки массива объектов остается на ваш выбор.
Для использования контрукции `unnest` допускается создавать пользовательский тип в базе только в схеме public (через миграции).

### Критерии приемки:

1. Присутствуют базы данных orders-1 и orders-2
2. В базе orders-1 находятся схемы bucket_0, bucket_1, bucket_2, bucket_3
3. В базе orders-2 находятся схемы bucket_4, bucket_5, bucket_6, bucket_7
4. В каждую схему bucket_N смигрирована структура БД
5. Реализован IShardingRule, выполняющий преобразование ключа шардирования в номер бакета
6. Реализован ShardConnectionFactory, создающий подключения к нужному инстансу БД на основе ключа
7. Источником данных для определения распределения бакетов по хостам в ShardConnectionFactory должен быть ServiceDiscovery
8. Репозитории переведены на использование ShardConnectionFactory
9. Все API работают как и прежде.

#### Для задания со звездочкой:
1. Создана индексная таблица для поиска заказов не по ключу шардирования. Количество индексов опрделеяется контрактами на поиск заказов (*)
2. При вставке (обновлении, затраигивающем индексное поле) заказа, также заносится/обновляется запись в индексную таблицу/ы (без транзакции)
3. Поиск заказов не по ключу шардирования производится через индексную таблицу/ы, а не по всем бакетам

**(*)** Для выполнения задания со звездочкой, достаточно реализовать индекс по регионам и использовать его для всех подходящих запросов.
Использование индекса для поиска заказов по клиенту остается на ваше усмотрение.

Дедлайн: 28 октября, 23:59 (сдача) / 31 октября, 23:59 (проверка)

## Домашнее задание пятой недели обучения

### Спроектировать базу данных orders для хранения заказов

* Обеспечить хранение данных заказа, регионов (совместно со складом или отдельно склады). Тип полей выбирать исходя из полей ваших моделей данных. Допустимо использовать jsonb/enum в случае необходимости. Товарный состав заказа сохранять не нужно.

* Создать слой миграций используя FluentMigrator. Миграция должна быть строго SQL, fluent-синтаксис недопустим. Миграция должна создавать БД и необходимые индексы. Необходимость индекса определяется самостоятельно.

* Реализовать интерфейсы репозиториев (существующие in-memory оставить). Все методы абстрактных репозиториев должны быть асинхронными (возвращать `Task<>`) и потдерживать отмену (`CancellationToken`)

* Удалять ранее реализованные методы API и/или методы репозиториев тоже нельзя.

Задание со звездочкой:
* Написать интеграционные тесты для репозиториев или целиком сервиса через gRPC API. Обеспечивать запуск интеграционных тестов в ci/cd не
  нужно.

Дедлайн: 21 октября, 23:59 (сдача) / 24 октября, 23:59 (проверка)

## Домашнее задание четвертой недели обучения

#### 1. Необходимо кэшировать ответы от сервиса CustomerService

* Реализовать кэширование через `IDistributedCache` или `StackExchange.Redis` (На воршкопе расматривался вариант только с StackExchange.Redis)

#### 2. Необходимо реализовать Consumer для топика pre_orders
* Получаем данные из топика `pre_orders`
* Обогащаем данными из сервиса CustomerService
* Надо обогатить данные так, что мы могли выполнять агрегацию данных
* Обогащенные данные сохраняем в репозиторий

#### 3. Валидация данных перед отправкой в new_orders
* Добавить кадому региону склад с координатами
* Координаты придумайте сами
* Проверяем расстояние между адресом в заказе и складом региона
* Если расстояние более 5000, то заказ не валидиный
  ** Там захардкожены только двое координат. (55.7522, 37.6156 и 55.01, 82.55)

* Заказы сохраняются независимо от валидности

#### 4. Необходимо реализовать Poducer для топика new_orders
* Валидные заказы необходимо отправлять в топик `new_orders`

#### 5. Необходимо реализовать Consumer для топика orders_events
* Читать сообщения из топика `orders_events`
* Обновлять статус заказа

** Контракт для топика `pre_orders`
key:orderId
value:
```json
{
    "Id": 82788613,
    "Source": 1,
    "Customer": {
        "Id": 1333768,
        "Address": {
            "Region": "Montana",
            "City": "East Erich",
            "Street": "Bernier Stream",
            "Building": "0744",
            "Apartment": "447",
            "Latitude": -29.8206,
            "Longitude": -50.1263
        }
    },
    "Goods": [
        {
            "Id": 5140271,
            "Name": "Intelligent Rubber Shoes",
            "Quantity": 6,
            "Price": 2204.92,
            "Weight": 2802271506
        },
        {
            "Id": 2594594,
            "Name": "Rustic Frozen Pants",
            "Quantity": 8,
            "Price": 1576.55,
            "Weight": 3174423838
        },
        {
            "Id": 6005559,
            "Name": "Practical Plastic Soap",
            "Quantity": 2,
            "Price": 1034.51,
            "Weight": 2587375422
        }
    ]
}
```

** Контракт для топика `new_orders`
key:orderId
value:
```json
{"OrderId": 1}
```

** Контракт для топика `orders_events`
key:orderId
value:
```json
{
	"Id": 20032,
	"NewState": "SentToCustomer",
    "UpdateDate": "2023-03-11T11:40:44.964164+00:00"
}
```

Задание со **\***

* Написать Unit тесты для новой логики

> **Дедлайн: 17 июня 23:59 (сдача) / 20 июня 23:59 (проверка).**