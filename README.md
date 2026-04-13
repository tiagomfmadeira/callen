# Callen

WPF desktop app to manage a calendar collection.

## Repo structure

- `Callen/` - Visual Studio solution and WPF project
- `Data/` - database scripts and local data folder
- `scripts/` - build and packaging scripts
- `dist/` - generated portable output (not source)

## Runtime paths

Default runtime paths are configured in `Callen/Callen/App.config`:

- `database_path = .\Data\Callen.db`
- `image_path = .\Images`

Users can change both paths in the Settings window.

## Data privacy in git

The repository ignores local data files:

- `Data/*.db`
- `Data/*.sqlite`
- `Data/table*.xml`

This keeps real database content and source XML out of the public repo.

## Create an empty database

Use this script to create a clean local DB with schema only:

```powershell
python .\Data\init_database.py
```

Optional custom output path:

```powershell
python .\Data\init_database.py --output .\Data\Callen.db
```

## Build portable folder

1. Build `Release` in Visual Studio.
2. Run:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Create-Portable.ps1
```

Output is created in `dist/CallenPortable`.

## GitHub releases

A workflow is included at `.github/workflows/release-portable.yml`.
When you push a tag like `v1.2.0`, it will:

1. Restore packages
2. Build `Release`
3. Run `Create-Portable.ps1`
4. Upload `dist/CallenPortable.zip` to the GitHub Release
