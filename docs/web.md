# Web UI

The web interface runs the 8085 core and assembler in WebAssembly so it can execute programs directly in the browser.

## Features
- Text editor for entering assembly code.
- Run button assembles source and executes it in the simulator.
- Displays the final value of the `A` register.

## Building and Running
```bash
dotnet run --project ui-web
```
This launches a development server; open the displayed URL in a browser to use the simulator.
