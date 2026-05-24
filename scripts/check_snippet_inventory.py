#!/usr/bin/env python3
"""Check that snippet docs, implementation files, tests, and index stay in sync."""

from __future__ import annotations

import re
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
DOCS_ROOT = ROOT / "docs" / "snippets"
INDEX_PATH = DOCS_ROOT / "README.md"
SOURCE_ROOTS = (
    ROOT / "src" / "DotnetBackendSnippets.Core",
    ROOT / "src" / "DotnetBackendSnippets.AspNetCore",
    ROOT / "src" / "DotnetBackendSnippets.EntityFrameworkCore",
    ROOT / "src" / "DotnetBackendSnippets.UnsafeSamples",
)

PATH_IN_BACKTICKS = re.compile(r"`((?:src|tests)/[^`]+)`")
MARKDOWN_LINK = re.compile(r"\[[^\]]+\]\(([^)]+)\)")

CATEGORY_OVERRIDES = {
    "Api": "api",
    "Async": "async",
    "Authentication": "authentication",
    "BackgroundServices": "background-services",
    "Caching": "caching",
    "Collections": "collections",
    "Configuration": "configuration",
    "DateAndTime": "date-and-time",
    "DependencyInjection": "dependency-injection",
    "Deployment": "deployment",
    "DtoMapping": "dto-mapping",
    "ErrorHandling": "error-handling",
    "FileHandling": "file-handling",
    "HealthChecks": "health-checks",
    "HttpClient": "http-client",
    "HttpClientFactory": "http-client-factory",
    "LanguageFeatures": "language-features",
    "Linq": "linq",
    "Logging": "logging",
    "Numbers": "numbers",
    "Observability": "observability",
    "OpenApi": "openapi",
    "Options": "options",
    "RateLimiting": "rate-limiting",
    "RegularExpressions": "regular-expressions",
    "Security": "security",
    "Serialization": "serialization",
    "Strings": "strings",
    "Testing": "testing",
    "TypeSystem": "type-system",
    "Utilities": "utilities",
    "Validation": "validation",
}


def main() -> int:
    errors: list[str] = []

    docs = sorted(DOCS_ROOT.glob("**/*.md"))
    index_text = INDEX_PATH.read_text(encoding="utf-8")

    referenced_paths = collect_referenced_paths(docs)
    errors.extend(check_referenced_paths_exist(referenced_paths))
    errors.extend(check_docs_have_implementation_and_tests(docs))
    errors.extend(check_doc_categories_are_indexed(index_text))
    errors.extend(check_source_categories_have_docs())

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    print("Snippet inventory check passed.")
    return 0


def collect_referenced_paths(docs: list[Path]) -> dict[Path, list[str]]:
    referenced_paths: dict[Path, list[str]] = {}

    for doc in docs:
        text = doc.read_text(encoding="utf-8")
        paths = PATH_IN_BACKTICKS.findall(text)
        referenced_paths[doc] = paths

    return referenced_paths


def check_referenced_paths_exist(referenced_paths: dict[Path, list[str]]) -> list[str]:
    errors: list[str] = []

    for doc, paths in referenced_paths.items():
        for relative_path in paths:
            if "*" in relative_path:
                if not list(ROOT.glob(relative_path)):
                    errors.append(f"{relative(doc)} references missing path pattern `{relative_path}`.")
                continue

            if not (ROOT / relative_path).exists():
                errors.append(f"{relative(doc)} references missing path `{relative_path}`.")

    return errors


def check_docs_have_implementation_and_tests(docs: list[Path]) -> list[str]:
    errors: list[str] = []

    for doc in docs:
        if doc == INDEX_PATH or doc.name.endswith("-coverage.md"):
            continue

        text = doc.read_text(encoding="utf-8")
        paths = PATH_IN_BACKTICKS.findall(text)
        has_src = any(path.startswith("src/") for path in paths)
        has_tests = any(path.startswith("tests/") for path in paths)

        if not has_src:
            errors.append(f"{relative(doc)} does not list an implementation path.")

        if not has_tests:
            errors.append(f"{relative(doc)} does not list a test path.")

    return errors


def check_doc_categories_are_indexed(index_text: str) -> list[str]:
    errors: list[str] = []
    linked_docs = {
        normalize_link_target(match.group(1))
        for match in MARKDOWN_LINK.finditer(index_text)
        if not match.group(1).startswith(("http://", "https://", "#"))
    }

    for category_dir in sorted(path for path in DOCS_ROOT.iterdir() if path.is_dir()):
        docs_in_category = sorted(category_dir.glob("*.md"))
        if not docs_in_category:
            errors.append(f"{relative(category_dir)} has no markdown files.")
            continue

        if not any(str(relative_to_docs(doc)).replace("\\", "/") in linked_docs for doc in docs_in_category):
            errors.append(f"{relative(category_dir)} is not linked from docs/snippets/README.md.")

    return errors


def check_source_categories_have_docs() -> list[str]:
    errors: list[str] = []

    for source_root in SOURCE_ROOTS:
        if not source_root.exists():
            continue

        for category in sorted(path for path in source_root.iterdir() if path.is_dir()):
            if category.name in {"bin", "obj"}:
                continue

            doc_category = CATEGORY_OVERRIDES.get(category.name, to_kebab_case(category.name))
            if not (DOCS_ROOT / doc_category).exists():
                errors.append(
                    f"{relative(category)} has no docs/snippets/{doc_category}/ category."
                )

    return errors


def normalize_link_target(target: str) -> str:
    return target.split("#", maxsplit=1)[0].lstrip("./").replace("\\", "/")


def to_kebab_case(value: str) -> str:
    parts = re.findall(r"[A-Z]+(?=[A-Z][a-z]|$)|[A-Z]?[a-z]+|\d+", value)
    return "-".join(part.lower() for part in parts)


def relative(path: Path) -> str:
    return path.relative_to(ROOT).as_posix()


def relative_to_docs(path: Path) -> Path:
    return path.relative_to(DOCS_ROOT)


if __name__ == "__main__":
    sys.exit(main())
