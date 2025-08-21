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

## Contributing
See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.
