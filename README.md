# Avalanche: Digitizing Ski Passes with NFC

Avalanche is a school project that consists of a mobile application and a set of microservices. The goal of the project is to digitize ski passes using NFC technology.

*Unfortunately, the goal of my course was not to create both an application and a backend server. Therefore, for the fun, I did. It might be far from perfect as i had to kinda spead run to provide something as quickly as possible, so that we could experiment with the mobile application.*

## How it works

Basically, users own one ticket per station. When a plan is purchased for a station, we either extend the existing ticket or create a new one with the corresponding validity.

To validate a ticket, there two actors are involved: the station and the client.

## Dependencies

| Library | Description | 
| --- | --- |
| [YARP](https://github.com/microsoft/reverse-proxy) | Reverse proxy |
| [gRPC](https://grpc.io/) | Communication between client and server |
| [CAP](https://cap.dotnetcore.xyz/) and [RabbitMQ](https://www.rabbitmq.com/) | Distributed messaging and event-driven microservices |
| [PostgreSQL](https://www.postgresql.org/) | Persistence |
| [MediatR](https://github.com/jbogard/MediatR) | Introduces pipelines for commands and queries |
| [FluentValidation](https://fluentvalidation.net/) | Model validations |
| [OpenIddict](https://github.com/openiddict/openiddict-core) | Identity server |

## Services

![image](https://github.com/stupside/avalanche-backend/assets/41454550/93dee2cb-3b4d-4de0-8785-46f287f41f2a)

Communications to the server is done with gRPC. Requests are made to YARP, witch will forward requests to the proper services. This is mainly because we didn't want to maintain multiple ports and ip addresses while developping the mobile application.

### Merchant

This service should actually be called Catalog. Anyway, its goal is to hold the different plans users can buy per station.

### Vault

This service only reacts to planned purchases by listening to events fired by the merchant service. When such an event is received, the Vault service creates a ticket or extends its validity.

### DRM

The service is messy and should not be called that way.

When a client wants to validate its ticket for a station, and their NFC tags discover each other, the station doesn't know the user, and the client doesn't know the station. This is because the client cannot send its UserId to the station as we cannot trust them. Everything has to be done in an isolated way. Stations should only trust our server.

So, here comes the DRM service. Its goal is to allow a station and a client to discover each other without communicating. As well as orchestrating the ticket validation process and sending validation step results in real-time.

#### Steps
1. To make it work, whenever the station detects a client via NFC, a call to the DRM service is made using its Station Id. 
2. Then the server creates a challenge intent and responds with a ChallengeId.
3. This ID can then be used by the station to connect to a gRPC stream which will send back validation events once an authenticated client makes the proper call to the DRM service. 
4. To do so, the station sends the ChallengeId to the client over the APDU protocol so that the client can connect to the gRPC stream too.

*This was done by implementing our own HostApduService so our client's phone can act as a Card. 
As we didn't have any NFC reader at the time we had to add a little button so we could switch to NFC Reader mode and intéract as a station with our client by sending APDU Commands.*

#### Discovery
As the client connects to its account, and the service is validating user tokens by doing introspection, the DRM service can authenticate the client and know its identity. The service can notify the station, start processing the user's ticket for the station attached to the challenge, until the validation's final step is reached wiss success or something has failed.

One big mistake I have made with the DRM service is that I used a pub/sub mechanism to challenge the tickets. But well, that was funny; I played around with CAP, RabbitMQ, Channels, and Streams.

#### Improvements and thougts

1. Channels are not closed when the validation is completed.
2. When the validation is completed, gRPC connections relative to the challenge are not terminated.
3. I used a pub/sub mechanism to validate tickets.
4. There is no payment. Once you make a purchase, an event is fired and consumed by the Vault service to create or extend an existing ticket.

