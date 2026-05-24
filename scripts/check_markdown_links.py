from __future__ import annotations

import re
import sys
import unicodedata
from pathlib import Path
from urllib.parse import unquote


ROOT = Path(__file__).resolve().parents[1]
MARKDOWN_FILES = [ROOT / "README.md", *sorted((ROOT / "docs").rglob("*.md"))]
LINK_PATTERN = re.compile(r"(?<!!)\[[^\]]+\]\(([^)]+)\)")
HEADING_PATTERN = re.compile(r"^(#{1,6})\s+(.+?)\s*$")
EXTERNAL_PREFIXES = ("http://", "https://", "mailto:", "#")


def github_anchor(text: str) -> str:
    cleaned = text.strip().lower()
    cleaned = "".join(ch for ch in cleaned if is_github_anchor_character(ch))
    cleaned = re.sub(r"\s+", "-", cleaned)
    return cleaned


def is_github_anchor_character(character: str) -> bool:
    if character.isspace() or character == "-":
        return True

    category = unicodedata.category(character)
    return category[0] in {"L", "M", "N"}


def anchors_for(path: Path) -> set[str]:
    counts: dict[str, int] = {}
    anchors: set[str] = set()

    for line in path.read_text(encoding="utf-8").splitlines():
        match = HEADING_PATTERN.match(line)
        if match is None:
            continue

        base = github_anchor(match.group(2))
        index = counts.get(base, 0)
        counts[base] = index + 1
        anchors.add(base if index == 0 else f"{base}-{index}")

    return anchors


def split_target(raw_target: str) -> tuple[str, str]:
    target = raw_target.strip()
    if " " in target:
        target = target.split(" ", 1)[0]
    target = target.strip("<>")
    path, separator, fragment = target.partition("#")
    return unquote(path), unquote(fragment) if separator else ""


def main() -> int:
    errors: list[str] = []
    anchor_cache: dict[Path, set[str]] = {}

    for markdown in MARKDOWN_FILES:
        text = markdown.read_text(encoding="utf-8")
        for line_number, line in enumerate(text.splitlines(), start=1):
            if line.rstrip() != line:
                errors.append(f"{markdown.relative_to(ROOT)}:{line_number}: trailing whitespace")

            for match in LINK_PATTERN.finditer(line):
                raw_target = match.group(1)
                if raw_target.startswith(EXTERNAL_PREFIXES):
                    continue

                target_path, fragment = split_target(raw_target)
                candidate = markdown if target_path == "" else (markdown.parent / target_path).resolve()

                if not candidate.exists():
                    errors.append(f"{markdown.relative_to(ROOT)}:{line_number}: missing link target {raw_target}")
                    continue

                if fragment:
                    anchors = anchor_cache.setdefault(candidate, anchors_for(candidate))
                    if fragment not in anchors:
                        errors.append(f"{markdown.relative_to(ROOT)}:{line_number}: missing anchor #{fragment} in {candidate.relative_to(ROOT)}")

    for error in errors:
        print(error)

    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
