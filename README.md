# MicroLab Simulator

A clean, modern, and extensible 8085 lab simulator designed for teaching and self‑study.

## Project Structure

- `core` – 8085 CPU core abstractions.
- `asm8085` – Assembler and disassembler.
- `io` – I/O bus and peripheral interfaces.
- `grader` – Headless runner and autograder CLI.
- `ui-desktop` – Desktop GUI (Avalonia).
- `ui-web` – Web/PWA interface.
- `assets` – Icons, themes, and sample programs.
- `labs` – Lab templates and tests.
- `docs` – User and teacher guides.
- `tests` – Unit and integration tests.

## Building

```bash
dotnet build
```

## Testing

```bash
dotnet test
```

## CLI Usage

The `ml85` tool can assemble and run programs, grade them against JSON specs, or export execution traces:

```bash
ml85 run program.8085 --entry 0x0000
ml85 grade spec.json
ml85 trace program.8085 --out trace.json
```

## Peripherals

The simulator includes a pluggable bus for external devices. The initial implementation provides an Intel 8255 PPI that can
be mapped to I/O ports for simple LED, switch, or seven‑segment display experiments.

## Contributing
See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.
