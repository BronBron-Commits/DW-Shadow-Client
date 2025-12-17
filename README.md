# Console Harness — Managed Wrapper

This repository contains a managed (.NET) console harness used to observe, intercept, and analyze runtime behavior of the DeltaWorlds client stack. It is designed as an investigative and experimental environment rather than a production client.

The project focuses on understanding protocol flow, state transitions, and cryptographic behavior by operating at the managed runtime level.

# Purpose

The primary goal of this harness is to make opaque client behavior observable.

Specifically, it is used to:

Study DeltaWorlds client initialization and login flow

Observe state-machine transitions during authentication

Intercept and inspect managed cryptographic operations

Experiment with runtime method patching prior to native execution

Provide a controlled environment for reverse engineering and documentation

This is not intended to be a drop-in replacement client, but a research tool that informs future client implementations.

# Architecture Overview

The harness is built as a .NET 8.0 x86 console application and acts as a managed interception layer.

Key architectural characteristics:

Managed-first execution model

Runtime method patching via Harmony

Cryptographic visibility via BouncyCastle interception

Deterministic startup sequence for patch attachment

Console-driven logging for timing and state observation

The design prioritizes traceability and inspection over performance or abstraction.

# Key Components

Managed wrapper project targeting net8.0 (win-x86)

Harmony-based patch scaffolding for runtime interception

BouncyCastle crypto hooks for observing encryption paths

Console harness for controlled execution and logging

Experimental structure intended to evolve alongside protocol research

# Intended Use

This repository is intended for:

Reverse engineering and protocol analysis

Runtime behavior research

SDK and client state-machine mapping

Cryptographic flow inspection

Supporting documentation and knowledge extraction

It is not intended for public distribution, gameplay, or commercial use.

Project Status

This project is in an early, experimental stage.

# Expect:

Breaking changes

Incomplete features

Rapid iteration

Exploratory code paths

Stability is not a goal at this stage.

Relationship to Other Work

This harness is part of a broader effort to document and understand the DeltaWorlds SDK and client behavior. Findings from this project feed into protocol documentation, client reimplementation efforts, and future rendering or VR-focused clients.

License and Disclaimer

This repository is for educational and research purposes only.

All trademarks, protocols, and technologies referenced belong to their respective owners. This project does not distribute proprietary assets or binaries.
