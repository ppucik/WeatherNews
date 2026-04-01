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

___

## Popis implementácie zadania:

Ide o vysoko dostupnú REST službu na poskytovanie informácií o teplote, postavenú s dôrazom na odolnosť (resilience), výkon a cloud-native štandardy v prostredí .NET 10.

### 📋 Technické špecifikácie

* **Architektúra:** Minimal APIs s využitím **Options patternu** a **Dependency Injection**.
* **Odolnosť:** Implementovaný `Microsoft.Extensions.Http.Resilience` (Polly) pre robustnú komunikáciu s externým API.
* **Caching:** Stratégia **Stale-While-Revalidate** pomocou `HybridCache` pre zabezpečenie dostupnosti dát aj pri výpadku externého zdroja.
* **Monitoring:** Health Checks (Liveness/Readiness) a natívne Metrics endpointy s vlastným JSON formátovaním.
* **Dokumentácia:** OpenAPI s interaktívnym **Scalar UI**.

---

### 🚀 Rýchly štart (Docker)

Aplikácia je plne kontajnerizovaná a optimalizovaná pre beh v prostredí Kubernetes (k8s).

#### 1. Spustenie cez Docker Compose
```bash
docker-compose up --build
```
#### 2. Manuálny Docker Build & Run
```bash
# Build obrazu
docker build -t weathernews-api .

# Spustenie API
docker run -p 8080:8080 -p 8081:8081 weathernews-api

# Spustenie s konfiguráciou JWT (Environment Variables)
docker run -p 8080:8080 \
  -e Auth__JwtKey="pripadne-vas-32-znakovy-tajny-kluc" \
  -e Auth__Issuer="weather-api" \
  -e Auth__Audience="weather-api-clients" \
  weathernews-api
```

---

### 🛠️ API Endpointy

| Metóda | Endpoint          | Popis                                    | Autentifikácia |
| ------ | ----------------- | ---------------------------------------- | -------------- |
| GET    | /                 | Základné informácie o verzii a prostredí | Nie            |
| GET    | /api/temperature/ | Aktuálna teplota v meste                 | Bearer JWT     |
| GET    | /health           | Health check aplikácie                   | Nie            |
| GET    | /metrics          | Základné metriky                         | Nie            |
| GET    | /scalar           | Interaktívna API dokumentácia            | Nie            |

---

### 🏥 Monitoring & Health Checks

Služba implementuje separátne sondy optimalizované pre cloudové prostredia:

- **Liveness** (`/health/live`)  
  Kontroluje, či proces aplikácie beží. V prípade zlyhania orchestrátor kontajner reštartuje.

- **Readiness** (`/health/ready`)  
  Hĺbková kontrola, ktorá overuje dostupnosť externého WeatherAPI.  
  Ak externá služba nie je dostupná, endpoint vráti detailný JSON so stavom **Unhealthy**  
  a orchestrátor dočasne prestane na túto inštanciu smerovať požiadavky.

- **Metrics** (`/metrics`)  
  Export systémových metrík (RAM, CPU, ThreadCount, Uptime) v prehľadnom JSON formáte.

---

### 🔑 Autentifikácia & Bezpečnosť

API je zabezpečené pomocou **JWT Bearer Tokenu**.

- **Development Mode:**  
  Pri spustení v prostredí Development aplikácia automaticky generuje platný DEV JWT token do konzoly (logs) pre okamžité testovanie.

- **Produkcia:**  
  Nastavenia sú plne konfigurovateľné cez `AuthOptions` pomocou Environment Variables alebo Secret Managerov.

---

### 🏗️ Architektonické rozhodnutia

1. **Resilience Pipeline:**  
  Komunikácia s externým API využíva inteligentný Retry mechanizmus.  
  Circuit Breaker chráni systém pred kaskádovým zlyhaním v prípade dlhodobého výpadku dodávateľa dát.

2. **Smart Caching:**  
  Dáta z WeatherAPI sa aktualizujú len v špecifických časoch (9:00 a 16:00 UTC).  
  `HybridCache` zabezpečuje, že aplikácia dokáže vrátiť *stale* (staršie) dáta, čím garantuje dostupnosť aj pri výpadku.

3. **Result Pattern:**  
  Namiesto nákladného vyhadzovania výnimiek pre bežné logické stavy (napr. neexistujúce mesto)  
  používame funkcionálny `Result<T>` wrapper.  
  To zvyšuje výkon (menej alokácií na stacku) a predvídateľnosť kódu.

4. **Clean Code & Separation of Concerns:**  
  `Program.cs` je udržiavaný v minimalistickom stave (*Thin Program.cs*).  
  Logika endpointov, konfigurácia a infraštruktúra sú striktne oddelené do samostatných modulov a extension metód.


```bash
docker build -t weathernews-test --no-cache . > build_log.txt 2>&1
```