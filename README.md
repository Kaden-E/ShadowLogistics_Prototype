# Shadow Logistics

A **systems-driven logistics strategy prototype** built in Unity.

Plan delivery routes across a network of towns and border crossings while managing escalating enforcement pressure, inspection risks, and economic growth. Every delivery alters the state of the world, creating a dynamic risk-reward simulation where expansion increases exposure.

See `CHANGELOG.md` for full development history.

---

## Current Version

**v0.8.1 — Contract Route Enforcement**

Shadow Logistics is currently a **vertical gameplay prototype** focused on systemic simulation and strategic decision making.

Latest systems include:

* Contract route enforcement
* Persistent regional **Heat accumulation**
* Dynamic **inspection pressure**
* **Contract tier progression**
* **Delivery outcome tracking**
* **Wallet-based economy**
* Data-driven systems built using **ScriptableObjects**

---

# Core Gameplay Loop

1. Select contracts from the board
2. Plan a route across connected towns and border crossings
3. Dispatch a vehicle to complete the delivery
4. Border inspections may occur depending on risk and regional pressure
5. Deliveries must reach the correct destination to succeed
6. Outcomes affect player money, reputation, and regional Heat

As operations expand, **risk exposure grows across the entire map**.

---

# Key Systems

## Contract System

Contracts define structured delivery objectives.

Each contract specifies:

* Origin town
* Destination town
* Cargo type
* Risk level
* Reward payout

Routes must begin at the contract origin and end at the contract destination to complete successfully.

A minimal **contract HUD** displays the active contract route and key information during gameplay.

---

## Route Planning System

Players manually construct delivery routes across a graph network.

* Towns and borders are graph nodes
* Routes must follow valid connections
* Player controls each segment of the journey
* Dispatch validation ensures routes start at the correct origin

This system gives players full control over their logistics operations.

---

## Border Inspection System

Each border crossing can trigger inspections.

Inspections include:

* Detection rolls
* Delay penalties
* Confiscation outcomes
* Severity-based consequences

Inspection probabilities dynamically scale with **regional Heat levels**.

---

## Persistent Regional Heat System

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

## Delivery Outcome System

Every delivery produces a structured result:

* Success
* Inspection delay
* Confiscation
* Failure
* Wrong-destination delivery

Delivery results update:

* Player wallet
* Regional Heat
* Delivery history logs

This system forms the backbone for the game's evolving world state.

---

## Vehicle and Cargo System

Vehicles define delivery capacity and behavior.

Cargo types include:

* Size classifications
* Configuration data
* Compatibility with vehicle types

Vehicle configuration is driven by **ScriptableObject data assets**, allowing flexible balancing and expansion.

---

# Architecture Overview

The project follows a **layered system architecture**:
