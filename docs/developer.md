# Developer Guide

This guide outlines the internal architecture of the MicroLab Simulator and the technologies used throughout the project.  It is intended for contributors who want to extend or understand the codebase.

## Technologies

- **.NET 8 / C#** – primary implementation language targeting Windows, macOS and Linux.
- **Avalonia UI** – cross‑platform desktop interface used by `ui-desktop`.
- **Blazor WebAssembly** – browser delivery of the simulator through `ui-web`.
- **GitHub Actions** – continuous integration running `dotnet build` and `dotnet test` on each commit.

## Solution Layout

```
micro-lab-sim.sln
├─ core/        – 8085 CPU library
├─ asm8085/     – assembler and disassembler
├─ io/          – system bus and peripherals (e.g. 8255)
├─ grader/      – headless runner and JSON autograder
├─ ui-desktop/  – Avalonia desktop shell
├─ ui-web/      – Blazor WebAssembly frontend
├─ labs/        – sample programs and grading specs
└─ tests/       – unit and integration tests
```

## Core Logic

### CPU Core (`core`)
Implements the 8085 step loop via `Cpu8085`.  Instructions fetch opcodes from an `IBus`, mutate registers in `CpuState`, update flags and return cycle counts.  The bus abstracts memory and port I/O enabling pluggable peripherals.

### Assembler (`asm8085`)
Parses classic 8085 assembly with labels and directives (`ORG`, `DB`, `END`) and emits machine code.  The same library provides a disassembler used in the UI and tests.

### I/O and Peripherals (`io`)
`SystemBus` dispatches memory and port accesses to registered devices.  The initial `Ppi8255` model demonstrates how peripherals integrate; additional devices can be added through the `IPeripheral` interface.

### Grader and Runner (`grader`)
The `ml85` CLI assembles source files, executes them against the core, and optionally grades results via JSON specs.  Execution traces and memory/port snapshots enable deterministic grading and lab automation.

### User Interfaces
- **Desktop (`ui-desktop`)** – Avalonia app with code editor, execution controls, register/flag/memory views, and widgets for LEDs, seven‑segment displays, and DIP switches.
- **Web (`ui-web`)** – Blazor WebAssembly app sharing the same core libraries to run entirely in the browser.

## Development Workflow

1. Restore and build the solution:
   ```bash
   dotnet build microlab-sim.sln
   ```
2. Run tests:
   ```bash
   dotnet test microlab-sim.sln
   ```
3. Add new peripherals or opcodes by extending the relevant libraries and covering them with tests in `tests/`.

## Coding Standards

The repository enforces formatting rules via `.editorconfig` and requires tests for new features.  See `CONTRIBUTING.md` for submission guidelines.

