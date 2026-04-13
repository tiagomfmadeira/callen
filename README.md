# Callen

[![Release](https://img.shields.io/github/v/release/dedukun/callen?display_name=tag)](https://github.com/dedukun/callen/releases)
[![Build](https://img.shields.io/github/actions/workflow/status/dedukun/callen/release-portable.yml?label=release)](https://github.com/dedukun/callen/actions/workflows/release-portable.yml)

WPF desktop app to manage a personal collection.

## Structure

- `src/Callen` - Visual Studio project
- `data` - local database scripts and local data files
- `scripts` - packaging scripts
- `dist` - generated artifacts
- `.github/workflows` - CI/release automation

## Build

1. Open `Callen.sln` in Visual Studio.
2. Build `Release`.

## Portable package

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Create-Portable.ps1
```

Output:

- folder: `dist/callen`
- zip (release asset): `dist/callen.zip`

## Local data scripts

Create empty DB:

```powershell
python .\data\init_database.py
```

Seed DB from XML:

```powershell
python .\data\seed_database.py
```
