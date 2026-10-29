# Architecture-as-docs (arc42 + C4)

Dokumentácia architektúry služby je uložená v repozitári, rozdelená na témy (jedna téma = jeden súbor),
diagramy sú ako kód (Mermaid) a verzované spolu so zdrojovým kódom.

## Prístup
- **C4 model** (Context, Container, Component, Code) – vrstvený pohľad od celku po detail.
- **arc42** – kontext, stavebné bloky, runtime view, deployment, koncepty, rozhodnutia, riziká, glosár.
- **ADR** – krátke záznamy „prečo“ (`adr/`).
- **DDD artefakty** – ubiquitous language, bounded context, agregáty, entity, value objects.
- **Executable documentation** – OpenAPI/Scalar, `.proto`, testy, EF migrácie.

## Best practices
- Najprv kontext a „prečo“, potom technické detaily.
- Ku každému tvrdeniu uvádzať odkaz na kód (`cesta:riadok`).
- Neisté časti označiť **ASSUMPTION** a zapísať do otvorených otázok.
- Dokumentovať aj existujúci stav vs. problémy (tech dlh, riziká) a sekciu „Pred refactoringom“.
- Iteratívne: vygenerovať → overiť s kódom/tímom → doplniť → ADR.

## Cieľová štruktúra
`00-overview.md`, `01-glossary.md`, `02-business-rules.md`, `03-workflows.md`, `04-architecture.md`,
`05-data-model.md`, `06-design-patterns.md`, `07-api-and-messaging.md`, `08-integrations-config.md`,
`09-cross-cutting.md`, `10-risks-tech-debt.md`, `11-refactoring-notes.md`, `adr/`.

Prompt na generovanie týchto dokumentov pomocou Claude Code: [claude-code-prompt.md](claude-code-prompt.md).
Prompt je generický – názov služby nahraď (napr. `WeatherNews`, `PaymentOrderService`).

## Otvorené otázky / ASSUMPTION
_Zatiaľ prázdne – dopĺňa sa pri generovaní dokumentov._
