# User Guide

This guide explains how to build and use the MicroLab Simulator as a learner or hobbyist.

## Prerequisites

- .NET 8 SDK installed on your system
- A terminal or command prompt

## Building the Simulator

Clone the repository and build the solution:

```bash
dotnet build microlab-sim.sln
```

## Command-Line Runner

The `ml85` CLI assembles and executes 8085 programs.

### Run a Program
```bash
ml85 run path/to/program.8085 --entry 0x0800
```
This assembles `program.8085`, starts execution at address `0x0800`, and prints final register and flag values.

### Grade a Lab
```bash
ml85 grade path/to/spec.json
```
The spec file provides the program, entry point, and expected results.  The command reports pass or fail.

### Trace Execution
```bash
ml85 trace path/to/program.8085 --out trace.json
```
Writes a JSON trace containing per-instruction program counters, registers, and memory diffs.

## Desktop UI

Run the Avalonia desktop interface:

```bash
dotnet run --project ui-desktop
```
Features include:
- Monaco-based code editor with assemble & run buttons
- Register, flag, memory, and port views
- Widget support for LEDs, DIP switches, and seven‑segment display

## Web UI

To launch the browser version during development:

```bash
dotnet run --project ui-web
```
Open the printed URL in a modern browser.  The web interface shares the same functionality as the desktop app and can be installed as a Progressive Web App.

## Sample Lab

A simple addition program is provided:

```bash
ml85 run labs/simple-add/simple-add.8085 --entry 0x0800
```
The program loads two numbers, adds them, and halts.  Use it to verify your setup.

