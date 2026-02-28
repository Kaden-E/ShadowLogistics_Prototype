Shadow Logistics

A graph-based logistics strategy prototype built in Unity.
Plan routes manually, manage border risk, and dispatch deliveries across a dynamic network.

Current Version

v0.2 – Routing & Border Inspection (Pre-Release)

Core Concept

Shadow Logistics is a systems-driven prototype focused on:

Player-authored route planning

Graph-based network traversal

Risk-based border crossings

Route visualization and simulation

The player manually constructs a route segment-by-segment across connected towns and border nodes, then dispatches a truck to simulate delivery.

Current Features
Graph-Based Routing

Towns and border crossings are unified as graph nodes

Routes must follow valid connections

Illegal paths are impossible (adjacency enforced)

Manual Route Planning

Click nodes to build route step-by-step

Right-click to undo last segment

ESC to cancel route

Space to dispatch

Truck Simulation

Truck travels along selected nodes

Speed varies by segment type

Border nodes introduce special events

Border Inspection System

Each border crossing has:

Speed multiplier

Inspection probability

Inspection delay

Inspections trigger:

Visual flashing feedback

Temporary delay

Debug toggle available to force inspections

Dynamic Network Visibility

Full route network hidden by default

Planned route segments highlighted during planning

Active route highlighted during delivery

Network hides again after completion

Architecture Overview
Planning Layer

TownManager

Builds player route

Validates adjacency

Controls route preview

Simulation Layer

RouteManager

Dispatches trucks

Traverses route nodes

Handles border inspection events

Graph Layer

Town

BorderNode (inherits from Town)

Connection

ConnectionManager

All routing logic is graph-driven.

Known Limitations

No economy system yet

No failure/heat mechanics (inspection only delays)

No UI panels (debug console used)

No save/load

Balance values not tuned

This is a systems prototype, not a full gameplay loop yet.

Roadmap
v0.3 (Planned)

Fuel cost per segment

Profit/loss calculation

Delivery summary

Risk escalation system

v0.4+

Heat system

Delivery failure states

UI overlays

AI competitors

Built With

Unity 2022 LTS

C#

Coroutine-based simulation

Graph-driven architecture

Status

Pre-release prototype.
Core routing systems stable.
Expanding toward full strategic loop.
