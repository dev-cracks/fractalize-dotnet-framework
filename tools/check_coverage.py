#!/usr/bin/env python3
"""Fails when the line coverage of the Cobertura reports under a folder is below a minimum.

Lines are merged by (source file, line number) across reports, so a line covered by any test
project counts as covered once. Usage: check_coverage.py <results-dir> [--min 80]
"""
import argparse
import pathlib
import sys
import xml.etree.ElementTree as ET


def collect(results_dir: pathlib.Path) -> dict[tuple[str, int], bool]:
    lines: dict[tuple[str, int], bool] = {}
    for report in results_dir.rglob("coverage.cobertura.xml"):
        for cls in ET.parse(report).getroot().iter("class"):
            filename = cls.get("filename", "")
            for line in cls.iter("line"):
                key = (filename, int(line.get("number", "0")))
                lines[key] = lines.get(key, False) or int(line.get("hits", "0")) > 0
    return lines


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("results_dir", type=pathlib.Path)
    parser.add_argument("--min", type=float, default=80.0, help="minimum line coverage in percent")
    args = parser.parse_args()

    lines = collect(args.results_dir)
    if not lines:
        print(f"::error::No coverage.cobertura.xml found under {args.results_dir}")
        return 1

    covered = sum(lines.values())
    percent = 100.0 * covered / len(lines)
    print(f"Line coverage: {percent:.2f}% ({covered}/{len(lines)} lines), minimum {args.min:.2f}%")
    if percent + 1e-9 < args.min:
        print(f"::error::Line coverage {percent:.2f}% is below the minimum of {args.min:.2f}%")
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
