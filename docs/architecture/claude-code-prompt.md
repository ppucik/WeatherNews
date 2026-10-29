# Prompt pre Claude Code (VS Code, root solution)

Nahraď `<SERVICE>` názvom služby (napr. `PaymentOrderService`) a doménové príklady upravte podľa služby.

```
Si senior .NET architekt. Analyzuj túto solution (microservice: <SERVICE>) a vytvor
dokumentáciu v /docs/architecture/ v Markdown s diagramami v Mermaid. Účel: aby som pochopil
fungovanie služby PRED úpravou a refactoringom.

Pravidlá:
- Vychádzaj iba z kódu, konfigurácie, migrácií a testov. Nič si nevymýšľaj; neisté veci označ
  "ASSUMPTION" a pridaj do zoznamu otvorených otázok.
- Ku každému dôležitému tvrdeniu uveď odkaz na súbor/triedu (cesta:riadok).
- Najprv preskúmaj: *.sln/*.slnx/*.csproj (projekty, závislosti, NuGet), Program.cs/Startup, DI registrácie,
  controllers/minimal APIs/gRPC, handlers (MediatR), domain modely, DbContext + migrácie,
  consumers/producers (MassTransit/Kafka/RabbitMQ/Service Bus), background services, appsettings,
  testy. Potom píš.
- Dokument píš po slovensky, technické názvy nechaj v origináli. Stručne, bez zbytočnej vaty.

Vytvor súbory:
1. 00-overview.md – účel služby, biznis kontext, hranice (čo robí / nerobí), kto ju používa,
   C4 Context + Container diagram.
2. 01-glossary.md – základné pojmy domény, ako sa používajú v kóde (názvy tried).
3. 02-business-rules.md – validácie, limity, schvaľovanie, výnimky, výpočty; tabuľka pravidlo →
   miesto v kóde.
4. 03-workflows.md – životný cyklus hlavnej entity (stateDiagram), sekvenčné diagramy pre hlavné
   scenáre (happy path, zamietnutie, retry, storno), idempotencia, chyby.
5. 04-architecture.md – štruktúra projektov, vrstvy, závislosti medzi nimi (diagram), C4 Component,
   tok requestu od endpointu po DB.
6. 05-data-model.md – entity, vzťahy (erDiagram), tabuľky, indexy, migrácie, transakcie, concurrency.
7. 06-design-patterns.md – použité patterny (CQRS, Repository, Mediator, Outbox, Saga, Strategy,
   Factory, Specification...), kde sa použili a prečo; anti-patterny.
8. 07-api-and-messaging.md – endpointy (request/response), udalosti/správy, kontrakty, verzovanie.
9. 08-integrations-config.md – externé systémy, konfigurácia, secrets, feature flags, deployment
   (Dockerfile, CI/CD).
10. 09-cross-cutting.md – autentifikácia/autorizácia, logovanie, audit, observability, error
    handling, resilience (Polly, retry), validácia.
11. 10-risks-tech-debt.md – code smells, duplicity, silné väzby, chýbajúce testy, bezpečnostné a
    výkonnostné riziká, zoradené podľa závažnosti.
12. 11-refactoring-notes.md – bezpečné body zmeny, hot-spots, odporúčané poradie refactoringu,
    čo najprv pokryť testami.
13. adr/ – retrospektívne ADR pre zistené kľúčové rozhodnutia (kontext, rozhodnutie, dôsledky).
14. README.md – index dokumentov + zoznam otvorených otázok a ASSUMPTION.

Postup: najprv mi ukáž plán a zoznam preskúmaných súborov, počkaj na môj súhlas, potom generuj
súbory po jednom. Na konci vykonaj self-review: over, že každý odkaz na kód existuje a diagramy
sú syntakticky správne.
```

Tip: pridaj do repozitára `CLAUDE.md` s build/test príkazmi a konvenciami; dokumentáciu po zmenách
aktualizuj rovnakým promptom (diff režim).
