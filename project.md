# Project Roadmap

## Phase 0 – Spec & Skeleton ✅
- [x] Repository structure
- [x] Coding standards
- [x] Continuous integration
- [x] CPU interfaces

## Phase 1 – Core & Assembler MVP ✅
- [x] Minimal 8085 CPU implementation
- [x] Basic assembler with ORG/DB/END and core opcodes
- [x] Golden test: `MVI A,1; ADI 1; HLT`

## Phase 2 – Desktop UI MVP ✅
- [x] Avalonia desktop shell
- [x] Editor and assemble/run pipeline
- [x] Registers/flags/memory views

## Phase 3 – Teaching Tools 🚧
- [x] CLI runner
- [x] Autograder
- [ ] Lab packs and sample programs
- [ ] Trace export and state diffs

## Phase 4 – Peripheral Plug-ins ⏳
- [ ] 8255 PPI and basic widgets
- [ ] Timers, keyboard/display controllers
- [ ] Serial devices

## Phase 5 – Web/PWA ⏳
- [ ] WASM build of core and assembler
- [ ] Browser UI shell
- [ ] PWA packaging
