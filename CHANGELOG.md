# Changelog

All notable changes to **Shadow Logistics** will be documented in this file.

The project follows a **version-based development structure**, where each version introduces a new systemic gameplay layer.

---

## v0.8.1 — Contract Route Enforcement

### Added

- Contract origin validation at route dispatch
- Destination validation at delivery completion
- Wrong-destination delivery failure handling
- Final node tracking in delivery results
- Delivery failure reason tracking
- Active contract HUD display

### Systems Introduced

- `ContractHUD`
- `DeliveryFailureReason`

### Systems Updated

- `ContractDefinition` now includes origin and destination fields
- `ActiveContractContext` now receives contract route data
- `RouteManager` now validates route start and destination
- `DeliveryResult` records final node and failure reason

### Impact

Contracts now function as **structured delivery objectives**.

Routes must begin at the contract origin and terminate at the contract destination to succeed.  
Incorrect routes are blocked at dispatch or fail on completion.

A minimal in-game HUD now displays the active contract route and relevant delivery information.

This patch restores contract integrity without introducing new gameplay systems.

---

## v0.8.0 — Persistent Regional Heat System

### Added

- Regional Heat tracking system
- Heat accumulation from inspections
- Heat spikes from caught deliveries
- Heat-based inspection probability scaling
- Heat decay after successful deliveries
- Region-based Heat persistence
- Minimal Heat HUD display

### Systems Introduced

- `HeatService`
- `HeatConfig`
- `HeatSaveData`
- `HeatHUD`

### Impact

Introduces long-term enforcement pressure across regions, transforming inspections from isolated events into a persistent strategic system.

---

## v0.7.0 — Delivery Outcome System

### Added

- Delivery result resolution system
- Delivery history logging
- Structured outcome states
- Wallet payout integration

### Systems Introduced

- `DeliveryOutcomeService`
- `DeliveryHistory`
- `DeliveryResult`

---

## v0.6.0 — Inspection Severity System

### Added

- Inspection severity bands
- Confiscation logic
- Fine calculation
- Data-driven inspection scaling

### Systems Introduced

- `InspectionSeverityBand`
- `InspectionSeverityCalculator`
- `InspectionSeverityConfig`

---

## v0.5.0 — Reputation Progression

### Added

- Reputation tracking
- Tier unlocking
- Contract progression system

---

## v0.4.0 — Contract Tier System

### Added

- Tier-based contract generation
- Tier reward multipliers
- Tier risk scaling

### Systems Introduced

- `ContractTier`
- `TierMultipliers`

---

## v0.3.0 — Economy Prototype

### Added

- Wallet system
- Delivery payout logic
- Basic profit tracking

### Systems Introduced

- `PlayerWallet`
- `WalletService`

---

## v0.2.0 — Routing & Border Inspections

### Added

- Manual route planning
- Border inspection triggers
- Graph-based map traversal

### Systems Introduced

- `RouteManager`
- `TownManager`
- `ConnectionManager`

---

## v0.1.0 — Vertical Map Prototype

### Added

- Initial country map
- Town nodes
- Graph-based connections
- Route visualization system