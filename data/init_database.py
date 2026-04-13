import argparse
import sqlite3
from pathlib import Path


SCHEMA = """
CREATE TABLE IF NOT EXISTS Archive (
    id INTEGER PRIMARY KEY,
    code TEXT,
    theme TEXT
);

CREATE TABLE IF NOT EXISTS Calendar (
    id INTEGER PRIMARY KEY,
    name TEXT,
    description TEXT,
    year TEXT,
    matrix TEXT,
    collection TEXT,
    date_inserted TEXT,
    date_modified TEXT,
    date_viewed TEXT,
    deleted INTEGER,
    pic_path TEXT,
    archive_id INTEGER,
    FOREIGN KEY (archive_id) REFERENCES Archive (id)
);
"""


def main() -> None:
    parser = argparse.ArgumentParser(description="Create an empty Callen SQLite database.")
    parser.add_argument(
        "--output",
        default=str(Path(__file__).with_name("Callen.db")),
        help="Output .db file path (default: data/Callen.db)",
    )
    args = parser.parse_args()

    output_path = Path(args.output).resolve()
    output_path.parent.mkdir(parents=True, exist_ok=True)

    with sqlite3.connect(str(output_path)) as conn:
        conn.executescript(SCHEMA)
        conn.commit()

    print(f"Empty database created: {output_path}")


if __name__ == "__main__":
    main()
