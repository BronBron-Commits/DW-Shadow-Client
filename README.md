DW-Shadow-Client™

DW-Shadow-Client™ is a low-level, socket-accurate research client for the DeltaWorlds authentication protocol.
It is designed to observe, validate, and replicate the server’s state machine behavior prior to login — not to bypass or emulate the full game client.

This project focuses on protocol correctness, not feature completeness.


---

Project Goals

Establish a byte-accurate baseline for the DeltaWorlds authentication handshake

Identify state transitions enforced by the server

Distinguish required, optional, and ignored protocol frames

Capture and analyze Phase-1 and Phase-2 server behavior

Build a foundation for future custom clients, tooling, and research


This is not:

A game client

A bot

A login bypass

A packet replay tool



---

Current Status (Verified)

✔ Implemented & Confirmed

TCP socket connection to auth.deltaworlds.com:6671

Byte-accurate ClientHello

Server Phase-1 envelope reception

Correct envelope parsing

total length

message type

flags

phase

reserved


Zlib header detection

Graceful handling of non-standard compression

Inflate attempted

Failure handled intentionally

Raw payload preserved


Phase-1 ACK requirement confirmed

ACK is mandatory

Missing or malformed ACK causes silent stall


Server state machine behavior observed

Invalid frames → Phase-1 replay

Incorrect phase advancement → no disconnect, no progress


Phase-2 challenge capture (read-only)

Phase-2 frames can be received

Payload is preserved without mutation


Capture-only flow is stable

No crashes

No undefined behavior

Deterministic server responses




---

What This Client Does Right Now

1. Connects to the DeltaWorlds auth server


2. Sends a verified ClientHello


3. Receives and parses the Phase-1 server envelope


4. Detects compression format and preserves payload


5. Sends a minimal, valid Phase-1 ACK


6. Observes server response:

Phase-1 replay or

Phase-2 challenge (depending on ACK correctness)



7. Logs and exits cleanly




---

What Is Not Implemented (Yet)

❌ Phase-2 challenge response

❌ Cryptographic response generation

❌ Session key derivation

❌ Username / password submission

❌ World or game state logic


These are intentionally out of scope until Phase-2 structure is fully understood.


---

Why This Matters

The DeltaWorlds authentication protocol is state-driven, not packet-driven.

DW-Shadow-Client™ proves:

You cannot skip states

You cannot replay server frames

You cannot brute-force forward

The server silently enforces protocol correctness


This project establishes the first verified, minimal client-side state progression outside of the official client.


---

Repository Structure

SocketShadow/
├── ShadowClient.cs              # Main socket client
├── TimingProfile.cs             # Observed timing behavior
├── HexDump.cs                   # Binary inspection helper
├── Protocol/
│   ├── AuthEnvelope.cs
│   ├── AuthEnvelopeDecoder.cs
│   ├── AuthFrameBuilder.cs
│   ├── Phase2LoginChallengeDecoder.cs
└── captures/                    # Raw protocol captures (ignored by git)


---

Development Philosophy

State machine first

Observe before modifying

Capture before responding

Fail safely

Never assume packet meaning without evidence


Every change is validated against live server behavior, not speculation.


---

Roadmap (High Level)

1. Finalize canonical Phase-1 ACK structure


2. Identify Phase-2 challenge components:

nonce

capabilities

server seed



3. Correlate Phase-2 payload with official client behavior


4. Implement Phase-2 response (research branch)


5. Transition to authenticated session (future)




---

Legal & Ethical Notice

This project is for research and interoperability study only.
No proprietary assets are redistributed.
No encryption is bypassed.
No server behavior is abused.


---

Name

DW-Shadow-Client™
Because it observes the protocol without pretending to be the client.
