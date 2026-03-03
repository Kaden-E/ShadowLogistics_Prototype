# Shadow Logistics

A **systems-driven logistics strategy prototype** built in Unity.

Plan delivery routes across a network of towns and border crossings while managing escalating enforcement pressure, inspection risks, and economic growth. Every delivery alters the state of the world, creating a dynamic risk-reward simulation where expansion increases exposure.

See CHANGELOG.md for full development history.

---

## Current Version

**v0.8.0 — Persistent Regional Heat System**

Shadow Logistics is currently a **vertical gameplay prototype** focused on systemic simulation and strategic decision making.

Latest systems include:

* Regional **Heat accumulation**
* Dynamic **inspection pressure**
* **Contract tier progression**
* **Delivery outcome tracking**
* **Wallet-based economy**
* Data-driven systems built using **ScriptableObjects**

---

## Core Gameplay Loop

1. Select contracts from the board
2. Plan a route across connected towns and border crossings
3. Dispatch a vehicle to complete the delivery
4. Border inspections may occur depending on risk and regional pressure
5. Outcomes affect player money, reputation, and regional Heat
6. Growing operations increase systemic pressure and enforcement activity

The player must balance **growth, profit, and exposure** while navigating a constantly evolving network.

---

## Key Systems

### Route Planning System

Players manually construct routes across a graph network.

* Towns and borders are graph nodes
* Routes must follow valid connections
* Player controls each segment of the journey
* Illegal routes are impossible

---

### Border Inspection System

Each border crossing can trigger inspections.

Inspections include:

* Detection rolls
* Delay penalties
* Confiscation outcomes
* Severity-based consequences

Inspection probabilities dynamically scale with **regional Heat levels**.

---

### Persistent Regional Heat System (v0.8.0)

Every region tracks enforcement pressure over time.

Heat increases from:

* Inspections
* Caught deliveries
* Failed smuggling attempts

Higher Heat results in:

* Increased inspection probability
* Higher severity outcomes
* Greater long-term strategic pressure

Heat decays slowly but never instantly resets, forcing players to manage risk across multiple regions.

---

### Contract Economy

Contracts are generated using a **tier-based system**.

Contracts define:

* Origin and destination
* Cargo type
* Risk level
* Reward payout

Higher tiers unlock stronger rewards but introduce greater systemic exposure.

---

### Delivery Outcome System

Every delivery produces a structured result:

* Success
* Inspection delay
* Confiscation
* Failure

Delivery results update:

* Player wallet
* Regional Heat
* Delivery history logs

This system forms the backbone for the game's evolving world state.

---

### Vehicle and Cargo System

Vehicles define delivery capacity and behavior.

Cargo types include:

* Size classifications
* Configuration data
* Compatibility with vehicle types

Vehicle configuration is driven by **ScriptableObject data assets**, allowing flexible balancing and expansion.

---

## Architecture Overview

The project follows a **layered system architecture**:

```
UI Layer
↓
Managers / Controllers
↓
Services
↓
Domain Models
↓
Data (ScriptableObjects)
```

Examples:

* `RouteManager` orchestrates route execution
* `DeliveryOutcomeService` resolves delivery results
* `WalletService` manages player finances
* `HeatService` tracks enforcement pressure
* `ContractManager` generates and manages contracts

Game logic is separated from configuration data to support scalable system growth.

---

## Current Systems Implemented

* Vertical map foundation
* Graph-based routing system
* Manual route planning
* Border inspection system
* Contract tier progression
* Reputation unlocking
* Data-driven economy
* Delivery outcome tracking
* Delivery history logging
* Persistent regional Heat system

---

## Technology

* **Unity 2022 LTS**
* **C#**
* **ScriptableObject data architecture**
* Graph-based world simulation
* Coroutine-driven delivery simulation

---

## Project Status

Shadow Logistics is an **actively evolving prototype**.

The project is focused on building a **systemic logistics simulation**, where player decisions dynamically influence enforcement pressure, economic opportunity, and long-term operational stability.

---

## Planned Systems

Future development aims to expand systemic depth:

* Dynamic regional instability
* AI competitors
* Supply chain disruptions
* Expanded economic simulation
* Strategic map overlays
* Save/load persistence

---

## Development Approach

The project follows an **incremental version architecture**, where each version introduces a new systemic layer.

Example progression:

```
v0.1  Map foundation
v0.2  Route planning
v0.3  Economy prototype
v0.4  Contract tiers
v0.5  Reputation progression
v0.6  Inspection severity system
v0.7  Delivery outcome system
v0.8  Persistent regional Heat
```

Each version expands the simulation rather than replacing previous systems.

---

## License

This project is currently distributed for **portfolio and development purposes**.
