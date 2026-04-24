import argparse
import re
import sqlite3
from datetime import datetime
from pathlib import Path
from typing import Dict, Iterable, Iterator, List, Optional, Set, Tuple
from xml.etree import ElementTree as ET

from init_database import SCHEMA


DEFAULT_THEME = "Tema não especificado"
DEFAULT_FOLDER_CODE = "Pasta não especificada"
THEME_REFERENCE_RE = re.compile(r"(?i)\btema(?:s|sa)\s+(?:da\s+)?pasta\s+([0-9]+(?:[oO])?)")


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    base_dir = Path(__file__).resolve().parent
    parser = argparse.ArgumentParser(description="Seed Callen.db from table1.xml and table2.xml.")
    parser.add_argument("--database", default=str(base_dir / "Callen.db"), help="Target SQLite database path.")
    parser.add_argument("--table1", default=str(base_dir / "table1.xml"), help="Path to table1.xml.")
    parser.add_argument("--table2", default=str(base_dir / "table2.xml"), help="Path to table2.xml.")
    return parser.parse_args()


def normalize_text(value: Optional[str]) -> str:
    """Return a trimmed string."""
    return (value or "").strip()


def normalize_code(value: Optional[str]) -> str:
    """Normalize folder codes by trimming and removing internal whitespace."""
    normalized = re.sub(r"\s+", "", normalize_text(value))
    return normalized if normalized else DEFAULT_FOLDER_CODE


def append_unique(values: List[str], value: str) -> None:
    """Append a value preserving order and uniqueness."""
    if value and value not in values:
        values.append(value)


def iter_rows(xml_path: Path) -> Iterator[Dict[str, str]]:
    """Yield each XML row as a dictionary keyed by tag name."""
    root = ET.parse(str(xml_path)).getroot()
    for row in root:
        yield {cell.tag: normalize_text(cell.text) for cell in row}


def extract_referenced_base_code(theme: str) -> Optional[str]:
    """Extract referenced base folder code from a 'Temas da Pasta X' theme label."""
    match = THEME_REFERENCE_RE.search(theme)
    if not match:
        return None
    token = match.group(1).replace("o", "0").replace("O", "0")
    return normalize_code(token)


def load_raw_themes_by_code(table2_path: Path) -> Dict[str, List[str]]:
    """Load direct themes from table2.xml keyed by folder code."""
    themes_by_code: Dict[str, List[str]] = {}
    for row in iter_rows(table2_path):
        code = normalize_code(row.get("PASTA"))
        theme = normalize_text(row.get("DESIGNAÇÃO"))
        if not code or not theme:
            continue
        themes_by_code.setdefault(code, [])
        append_unique(themes_by_code[code], theme)
    return themes_by_code


def resolve_concrete_themes(raw_themes_by_code: Dict[str, List[str]]) -> Dict[str, List[str]]:
    """Resolve concrete themes for each folder, expanding 'Temas da Pasta X' references."""
    cache: Dict[str, List[str]] = {}

    def resolve_for_code(code: str, stack: Set[str]) -> List[str]:
        if code in cache:
            return list(cache[code])
        if code in stack:
            return []

        stack.add(code)
        resolved: List[str] = []
        for theme in raw_themes_by_code.get(code, []):
            base_code = extract_referenced_base_code(theme)
            if base_code:
                for derived_theme in resolve_for_code(base_code, stack):
                    append_unique(resolved, derived_theme)
                continue
            append_unique(resolved, theme)
        stack.remove(code)
        cache[code] = resolved
        return list(resolved)

    for code in raw_themes_by_code:
        resolve_for_code(code, set())

    return cache


def themes_for_archive(concrete_themes: List[str]) -> List[str]:
    """Return archive themes with the default theme always present."""
    themes = [DEFAULT_THEME]
    for theme in concrete_themes:
        append_unique(themes, theme)
    return themes


def assignment_theme_for_code(concrete_themes: List[str]) -> str:
    """Choose the theme used by calendar rows for a given folder."""
    if len(concrete_themes) == 1 and concrete_themes[0] != DEFAULT_THEME:
        return concrete_themes[0]
    return DEFAULT_THEME


def insert_archives(
    conn: sqlite3.Connection,
    concrete_themes_by_code: Dict[str, List[str]],
) -> Tuple[Dict[Tuple[str, str], int], Dict[str, str]]:
    """Insert archive rows and return id and assignment mappings."""
    archive_id_by_code_theme: Dict[Tuple[str, str], int] = {}
    assignment_theme_by_code: Dict[str, str] = {}

    cursor = conn.cursor()
    for code, concrete_themes in concrete_themes_by_code.items():
        assignment_theme_by_code[code] = assignment_theme_for_code(concrete_themes)
        for theme in themes_for_archive(concrete_themes):
            cursor.execute("INSERT INTO Archive(code, theme) VALUES(?, ?)", (code, theme))
            archive_id_by_code_theme[(code, theme)] = int(cursor.lastrowid)

    return archive_id_by_code_theme, assignment_theme_by_code


def iter_calendar_rows(
    table1_path: Path,
    archive_id_by_code_theme: Dict[Tuple[str, str], int],
    assignment_theme_by_code: Dict[str, str],
) -> Iterable[Tuple[int, str, str, str, str, str, str, str, str, int, str, int]]:
    """Yield calendar rows ready for insertion."""
    now = datetime.now().isoformat(timespec="seconds")
    for row in iter_rows(table1_path):
        code = normalize_code(row.get("PASTA"))
        assignment_theme = assignment_theme_by_code.get(code, DEFAULT_THEME)
        archive_id = archive_id_by_code_theme.get((code, assignment_theme))

        if archive_id is None:
            raise ValueError(f"Missing archive mapping for code '{code}' and theme '{assignment_theme}'.")

        try:
            calendar_id = int(normalize_text(row.get("ID")))
        except ValueError as exc:
            raise ValueError(f"Invalid calendar ID for folder '{code}': {row.get('ID')!r}") from exc

        yield (
            calendar_id,
            normalize_text(row.get("DESIGNAÇÃO")),
            normalize_text(row.get("DESIGNAÇÃO2")),
            normalize_text(row.get("ANO")),
            normalize_text(row.get("MATRIZ")),
            normalize_text(row.get("COLECÇÃO")),
            now,
            now,
            now,
            0,
            "",
            archive_id,
        )


def seed_database(database_path: Path, table1_path: Path, table2_path: Path) -> Tuple[int, int]:
    """Seed the database from XML files."""
    if not table1_path.exists():
        raise FileNotFoundError(f"Missing XML file: {table1_path}")
    if not table2_path.exists():
        raise FileNotFoundError(f"Missing XML file: {table2_path}")

    concrete_themes_by_code = resolve_concrete_themes(load_raw_themes_by_code(table2_path))
    codes_in_table1 = {normalize_code(row.get("PASTA")) for row in iter_rows(table1_path)}
    for code in codes_in_table1:
        concrete_themes_by_code.setdefault(code, [])

    with sqlite3.connect(str(database_path)) as conn:
        conn.executescript(SCHEMA)
        archive_id_by_code_theme, assignment_theme_by_code = insert_archives(conn, concrete_themes_by_code)
        calendar_rows = list(iter_calendar_rows(table1_path, archive_id_by_code_theme, assignment_theme_by_code))
        conn.executemany(
            """
            INSERT INTO Calendar(
                id, name, description, year, matrix, collection,
                date_inserted, date_modified, date_viewed, deleted, pic_path, archive_id
            ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """,
            calendar_rows,
        )

    return len(archive_id_by_code_theme), len(calendar_rows)


def main() -> None:
    args = parse_args()
    database_path = Path(args.database).resolve()
    table1_path = Path(args.table1).resolve()
    table2_path = Path(args.table2).resolve()
    database_path.parent.mkdir(parents=True, exist_ok=True)

    archive_count, calendar_count = seed_database(database_path, table1_path, table2_path)
    print(f"Database seeded: {database_path}")
    print(f"Archives inserted: {archive_count}")
    print(f"Calendars inserted: {calendar_count}")


if __name__ == "__main__":
    main()
