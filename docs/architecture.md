# Architecture Overview

This repository is a product monorepo derived from the CheeseGrater platform.

- Backend and web code are orchestrated via Nx.
- CheeseGrater is treated as the upstream platform.
- This repo (Hellverse) is downstream and product-specific.
- Patterns may be promoted upstream only after being proven in production code.

The `engine/` directory contains the Godot game client and is intentionally
excluded from Nx orchestration, caching, and project graph analysis.
