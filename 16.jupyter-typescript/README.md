# Jupyter Notebook with TypeScript

This project demonstrates how to run TypeScript in a Jupyter Notebook using the [tslab](https://github.com/yunabe/tslab) kernel, with a Docker setup for easy startup.

The included notebook and library solve a common `console.log` problem: when logging large amounts of data in a Jupyter notebook (or Node.js), output can slow down the browser. `pathConsoleLog` buffers logs after a configurable limit and flushes them in batches.

For more information check my blog: (blog link TBD)

## Files

| File | Description |
|------|-------------|
| `console-log-problem.ipynb` | Jupyter notebook (TypeScript kernel) demonstrating the buffered console.log |
| `lib.ts` | Library with `pathConsoleLog` — patches `console.log` to limit and batch output |
| `Dockerfile` | Jupyter + Node.js 24 + tslab (TypeScript kernel) |
| `docker-compose.yml` | One-command startup for the Jupyter server on port 8118 |
| `package.json` | Node.js dependencies (lodash, chalk, ts-node, ts-node-dev) |
| `tsconfig.json` | TypeScript configuration |

## Quick start with Docker

```sh
docker compose up --build
```

Then open http://localhost:8118 in your browser and open `console-log-problem.ipynb`.

## Manual setup

Requires Node.js 18+ and Jupyter Notebook (with Python 3.8+).

```sh
npm install
npm run dev
```

## Usage

```typescript
import { pathConsoleLog } from "./lib"

// Limit output to 100 lines, then buffer and flush every 10ms
pathConsoleLog(100, 10)
```

The function patches `console.log` to:

- Write the first N lines directly (default: 100)
- Buffer subsequent output and flush every few ms (default: 10ms)
- Print each log line with ANSI colors via Node's `util.inspect`

## Dependencies

- `tslab` — TypeScript kernel for Jupyter
- `lodash` — general utilities
- `chalk` — terminal string coloring
- `ts-node` / `ts-node-dev` — TypeScript execution engine
- `dotenv` — environment variable loading

## Checkout only this project

```sh
git clone --no-checkout https://github.com/KeesCBakker/keestalkstech-code-gallery.git
cd keestalkstech-code-gallery
git sparse-checkout init
git sparse-checkout set --no-cone 16.jupyter-typescript
git checkout main
cd 16.jupyter-typescript
ls
```
