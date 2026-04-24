[![Version](https://img.shields.io/github/v/release/tiagomfmadeira/callen?display_name=tag)](https://github.com/tiagomfmadeira/callen/releases)
[![CI](https://img.shields.io/github/actions/workflow/status/tiagomfmadeira/callen/release-portable.yml?label=ci)](https://github.com/tiagomfmadeira/callen/actions/workflows/release-portable.yml)
![C%23](https://img.shields.io/badge/language-C%23-239120)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4)
![Windows](https://img.shields.io/badge/platform-Windows-0078D6)

<p align="center">
  <img src="src/Callen/Icons/logo.svg" alt="Callen" width="150"><br>
  <b><span style="font-size: 20px;">Callen</span></b>
</p>

A WPF desktop application for organizing and managing a personal collection, with local database support, native Windows printing, and automated portable packaging.

## Structure

- `src/Callen` - Visual Studio project
- `data` - local database scripts and local data files
- `scripts` - packaging scripts
- `dist` - generated artifacts
- `.github/workflows` - CI/release automation

## Local data scripts

Create empty DB:

```powershell
python .\data\init_database.py
```

Seed DB from XML:

```powershell
python .\data\seed_database.py
```
