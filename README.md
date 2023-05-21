# FinancialDataApp

FinancialDataApp application uses event driven architecture + REST in order to fullfill the requrements. Pulled market data is saved to the database and streamed to subscribed clients (using RabbitMQ + MassTransit + SignalR). It consists of 3 custom microservices and 5 commercial ones.

Custom ones:
1. DataPullService - Worker service which uses scheduled job (https://www.quartz-scheduler.net/) to pull market data periodically (https://polygon.io/). Retrieved data is saved to SQL Server.
2. OutboxService - Worker service which is implementing outboxing pattern. It simply reads events from an outbox table and pushes them to Rabbit MQ broker
3. FinancialDataApi - REST API for getting/subscibing to market data.

External:
1. RabbitMq - Message broker
2. SQL Server Database - Relational database
3. Elasticsearch - NoSQL used for logs and tracing
4. Kibana - ES visualization
5. Jaeger - Trace visualization

Diagram:
![Alt text](https://github.com/stanislav-stoychev/FinancialDataApp/blob/master/Diagram.png?raw=true "Diagram")

Running the application requires having RMQ, ES, SQL Server, Kibana and Jaeger instances. If not present at given machine there is a option to use docker. (Docker files and docker-compose are included in the solution)

Instructions for starting up from docker:
1. If not using Windows or don't have C drive, adjust docker-compose file accordingly. 
2. Run: docker-compose build
3. Run: docker-compose up

Notes: 
1. Please wait severeal seconds before using the app, because startup orchestration is not implemented due to time constraints. 
2. Errors might be observed in logs during startup, because of components starting out of order.

Links using defaut config:
1. Swagger: https://localhost:8001/swagger/index.html
2. Ws: ws://localhost:8000/news-feed (Example message for connecting: "{"protocol":"json","version":1}", subscribing: "{"arguments":["AAPL"],"target":"Subscribe","type":1}")
3. RabbitMq: http://localhost:15672/
4. Kibana: http://localhost:5601/
5. Jaeger: http://localhost:16686/

Alternative solution would be replacing components/tehcnologies with different ones

1.1. Switch RMQ for Kafka Pros:
  - Faster streaming
  - Has retention period for messages

1.2. Switch RMQ for Kafka Cons:
  - Pull based conusmers
  - No brokering capabilities

2.1. Switch SignalR for GRPC Pros:
  - Faster data transfer
  - Better scaling

2.2. Switch SignalR for GRPC Cons:
  - Harder to use
