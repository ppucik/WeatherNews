# WeatherNews

## Zadanie .NET Senior Developer

**Jazyk:** `C#`  
**Framework:** `.NET 10`  

Vytvor samostatnu REST Web API sluzbu, ktora bude vracat aktualnu teplotu v mestach: GET /api/temperature/{city}

- Dostupne mesta su: "Bratislava", "Praha", "Budapest", "Vieden"

- Teplota je udavana v 2 desatinnych miestach

- Pre ziskanie aktualnej teploty vyuzi API (WeatherAPI) dostupne na vzorovej fake URL: GET https://nejakepocasie.net/{cityId} 

	- kde cityId je integer hodnota miest v nasej aplikacii:
        ```json
        1 => "bratislava"
        2 => "praha"
        3 => "budapest"
        4 => "vieden"
        ```
    - moze vratit standardne responses (200,404,5xx ... atd) a v pripade HTTP 200 vrati: { "temperatureC": 12.3, "measuredAtUtc": "2026-02-13T08:55:00Z" }

- Teplota sa meni (je dostupna potencionalne nova hodnota z WeatherAPI) len 2x do dna, a to o 9:00am a 16:00 UTC

- Ako kazda externa sluzba, aj toto WeatherAPI ma svoje vypadky, t.j. treba zabezpecit, aby sa neprenasali do nasej aplikacie, t.j. aby aplikacia vracala vzdy posledne vratenu/nameranu hodnotu z WeatherAPI

- Aplikacia bude nasadena vo viacero instanciach a znacne vytazovana pre ziskanie poslednej teploty jednotlivych podporovanych miest.

- Aplikacia bude spristupnena ako API, preto je dolezite poskytnut standardizovanu dokumentaciu a zabezpecnie pomocou Bearer Tokenu.

- A radi vieme co sa deje / nedeje, takze potrebujeme logovat.

- Testy su optional.

- Pouzivame k8s a docker.


> dotnet dev-certs https --trust

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZGV2LXVzZXIiLCJleHAiOjE3NzQ0ODI4NDQsImlzcyI6IndlYXRoZXItYXBpIiwiYXVkIjoid2VhdGhlci1hcGktY2xpZW50cyJ9.ZClhUtNuXENGSaRJzcbt-M_P2izdy2u8hPp3USLuaQk

Ako API spustíš v Dockeri:

1) Build:
> docker build -t weathernews-api .

2) Run:
> docker run -p 8080:8080 -p 8081:8081 weathernews-api

Ako pristúpiť k API a Scalar UI v Dockeri

API:
> http://localhost:8080/api/temperature/Bratislava

Scalar UI:
> http://localhost:8080/scalar

JWT v Dockeri

> docker run \
  -e Auth__JwtKey="super-secret" \
  -e Auth__Issuer="weather-api" \
  -e Auth__Audience="weather-api-clients" \
  -p 8080:8080 \
  weathernews-api

Ako otestovať Scalar v Dockeri

> http://localhost:8080/scalar

> GET /api/temperature/{city}