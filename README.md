# Redis Session State Library DEMO

Demo Project for ISP Session Core 10.0 or higher.
demonstrating the minimal setup and requirements for installing and using Isp Session
for help and registration for your license navigate to
https://www.nieropcomputervision/ispsession


## License

This software is licensed under the terms found in the LICENSE.txt file located in the project root. Please refer to the LICENSE file for the full text of the license.

Copyright © 2024 Nierop Computervision. All rights reserved.

## Product Description
Isp Session provides a mechanism, utilizing cookies, headers, or even IP addresses**, to maintain state affinity in your REST API or ASP.NET Core 6+ driven website. It facilitates application-level caching by combining Application State and Session State, which can be utilized together or independently. The added value over direct usage of Redis or MemCached is scalability. Each request has minimal backend interaction while ensuring full encryption and minimal data size. Additionally, Application State offers the capability to capture expiration events for background data refresh.

** We adhere to privacy laws by implementing obfuscation for IP addresses when this feature is utilized.

## Overview

The Redis Session State Library offers a robust and efficient solution for session state management in .NET Core 6+ applications, leveraging Redis 4 or later as the backend. Our solution employs advanced data projection techniques for encrypting the session state, securing your data at all times.

## History
Isp Session debuts at version 10.0, following the author's previous releases of Isp Session/Asp Session for classic ASP pages, which reached version 8.2. Available at https://github.com/egbertn/ispsession.io, Isp Session has been completely rewritten from scratch to leverage the best features of .NET 6.0, incorporating best practices learned from previous versions.

## Features

- **Secure Session State Management**: Encrypts session data using advanced data projection techniques.
- **High Performance**: Utilizes Redis 4+ for fast and reliable session state storage.
- **.NET Core 6+ Support**: Specifically designed for .NET Core 6 applications to ensure compatibility and optimal performance.
- **Privacy First Approach**: Operates without telemetry or external server connections, keeping all customer data private.

## Getting Started
To clone our demo from GitHub, use:
```shell
git clone https://github.com/egbertn/ispsession.core
```

If Redis is already running on your system with default settings, IspSession should connect seamlessly. Otherwise, for custom settings like a required password, modify the connection string in appsettings.Development.json. For example:

"ConnectionStrings": {
    "IspSession": "localhost:6379,ssl=False,defaultDatabase=3"
},

Start the site in debugging mode and note the chosen port from the console output, e.g.:

```text
info: Microsoft.Hosting.Lifetime[14] Now listening on: https://127.0.0.1:7058
info: Microsoft.Hosting.Lifetime[14] Now listening on: http://127.0.0.1:5045
```

Use curl to test the connection:

```shell
curl https://localhost:7058/counterwithapp
```

You should receive a response similar to:

```json
{
  "sessionCounter": 1,
  "isNewSession": true,
  "isExpiredSession": true,
  "sessionId": "39ecbd08-662e-4123-95e8-fc559446c73d",
  "appCounter": 2
}
```
Using a browser like Firefox should set and return a cookie, and refreshing the URL should increment both the sessionCounter and appCounter.

Note: Isp Session supports unlimited testing, development, and staging as long as the remote IP originates from a private network. License requirements are enforced upon deployment or when proxies are detected.

### Prerequisites

- .NET Core 6.0 or later
- Redis 4.0 or later

### Installation

To install the Redis Session State Library, execute the following NuGet command:

```shell
dotnet add package NCV.ISPSession
```

### Licensing / purchasing ISP Session
- Licenses are issued annually for external websites using non-loopback and public IP addresses.
- Development and testing typically do not require a license.
- A perpetual license option is available.
- Source code access may be granted under a signed NDA.
- License modifications during the subscription period can be managed via our website.

### How ISPSession works internally

ISP Session optimizes performance through techniques like handling numeric endianness, minimal .NET memory heap impact, built-in ASP.NET Core DataProtection integration, AES encryption for Redis keys, and efficient storage for complex and simple data types. It employs an optimistic concurrency strategy to prevent data overwrites and writes session state changes only when necessary, ensuring efficient data management and security.


- SessionState
  Manages session variables as a single blob, supporting optional compression for large data sets.

- ApplicationState:
  Activated via configuration, storing variables individually to optimize performance and support variable-specific expiration events.