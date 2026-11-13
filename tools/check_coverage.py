#!/usr/bin/env python3
"""Fails when the line coverage of the Cobertura reports under a folder is below a minimum.

Lines are merged by (absolute source file, line number) across reports, so a line covered by any
test project counts as covered once. Usage: check_coverage.py <results-dir> [--min 80]
"""
import argparse
import pathlib
import posixpath
import sys
from pathlib import PureWindowsPath
import xml.etree.ElementTree as ET


def _normalise(path: str) -> str:
    return posixpath.normpath(path.replace("\\", "/"))


def _absolute(filename: str, sources: list[str]) -> str:
    """Resolves a class filename against the <source> roots of its report.

    Each report has its own roots (coverlet uses the common folder of the instrumented assemblies),
    so the same file can appear under different relative names in different reports. Resolving it
    makes the key unique: a line is neither counted twice nor confused with another file of the same
    name.
    """
    name = _normalise(filename)
    if posixpath.isabs(name) or PureWindowsPath(name).is_absolute() or not sources:
        return name
    candidates = [_normalise(posixpath.join(_normalise(source), name)) for source in sources]
    found = next((path for path in candidates if pathlib.Path(path).is_file()), None)
    if found is not None:
        return found
    if len(candidates) > 1:
        # Without the sources on disk the right root is unknown: the most specific one (the longest)
        # is the safest guess, and the warning makes the ambiguity visible.
        print(f"::warning::{filename} is not under any of {sources}; using the most specific root")
    return max(candidates, key=len)


def collect(results_dir: pathlib.Path) -> dict[tuple[str, int], bool]:
    lines: dict[tuple[str, int], bool] = {}
    for report in results_dir.rglob("coverage.cobertura.xml"):
        root = ET.parse(report).getroot()
        sources = [source.text or "" for source in root.iter("source") if source.text]
        for cls in root.iter("class"):
            filename = _absolute(cls.get("filename", ""), sources)
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
