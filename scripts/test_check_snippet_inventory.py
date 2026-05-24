from __future__ import annotations

import tempfile
import unittest
from collections.abc import Callable
from pathlib import Path
from unittest.mock import patch

import check_snippet_inventory


class SnippetInventoryTests(unittest.TestCase):
    def test_docs_require_test_paths_when_implementation_path_exists(self) -> None:
        with TemporarySnippetRepository() as repository:
            doc = repository.write_doc(
                "strings/string-samples.md",
                "`src/DotnetBackendSnippets.Core/Strings/StringSamples.cs`\n",
            )

            errors = repository.run_with_patched_paths(
                lambda: check_snippet_inventory.check_docs_have_implementation_and_tests([doc])
            )

        self.assertEqual(
            ["docs/snippets/strings/string-samples.md does not list a test path."],
            errors,
        )

    def test_referenced_paths_must_exist(self) -> None:
        with TemporarySnippetRepository() as repository:
            doc = repository.write_doc(
                "strings/string-samples.md",
                "`src/DotnetBackendSnippets.Core/Strings/MissingSamples.cs`\n",
            )

            errors = repository.run_with_patched_paths(
                lambda: check_snippet_inventory.check_referenced_paths_exist(
                    {doc: ["src/DotnetBackendSnippets.Core/Strings/MissingSamples.cs"]}
                )
            )

        self.assertEqual(
            [
                "docs/snippets/strings/string-samples.md references missing path "
                "`src/DotnetBackendSnippets.Core/Strings/MissingSamples.cs`."
            ],
            errors,
        )

    def test_source_category_requires_docs_category(self) -> None:
        with TemporarySnippetRepository() as repository:
            repository.write_source("DotnetBackendSnippets.Core/Strings/StringSamples.cs")

            errors = repository.run_with_patched_paths(
                check_snippet_inventory.check_source_categories_have_docs
            )

        self.assertEqual(
            [
                "src/DotnetBackendSnippets.Core/Strings "
                "has no docs/snippets/strings/ category."
            ],
            errors,
        )

    def test_docs_category_must_be_linked_from_index(self) -> None:
        with TemporarySnippetRepository() as repository:
            repository.write_doc("strings/string-samples.md", "# 文字列操作\n")
            repository.write_index("# スニペット索引\n\n")

            errors = repository.run_with_patched_paths(
                lambda: check_snippet_inventory.check_doc_categories_are_indexed(
                    repository.index_path.read_text(encoding="utf-8")
                )
            )

        self.assertEqual(
            ["docs/snippets/strings is not linked from docs/snippets/README.md."],
            errors,
        )

    def test_category_overrides_map_source_category_to_docs_category(self) -> None:
        with TemporarySnippetRepository() as repository:
            repository.write_source("DotnetBackendSnippets.Core/DateAndTime/DateAndTimeSamples.cs")
            repository.write_doc("date-and-time/date-and-time-samples.md", "# 日付操作\n")

            errors = repository.run_with_patched_paths(
                check_snippet_inventory.check_source_categories_have_docs
            )

        self.assertEqual([], errors)


class TemporarySnippetRepository:
    def __enter__(self) -> TemporarySnippetRepository:
        self._temporary_directory = tempfile.TemporaryDirectory()
        self.root = Path(self._temporary_directory.name)
        self.docs_root = self.root / "docs" / "snippets"
        self.index_path = self.docs_root / "README.md"
        self.source_roots = (
            self.root / "src" / "DotnetBackendSnippets.Core",
            self.root / "src" / "DotnetBackendSnippets.AspNetCore",
            self.root / "src" / "DotnetBackendSnippets.EntityFrameworkCore",
            self.root / "src" / "DotnetBackendSnippets.UnsafeSamples",
        )

        self.docs_root.mkdir(parents=True)
        for source_root in self.source_roots:
            source_root.mkdir(parents=True)

        self.write_index("# スニペット索引\n\n")
        return self

    def __exit__(self, *args: object) -> None:
        self._temporary_directory.cleanup()

    def write_doc(self, relative_path: str, content: str) -> Path:
        path = self.docs_root / relative_path
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(content, encoding="utf-8")
        return path

    def write_index(self, content: str) -> Path:
        self.index_path.write_text(content, encoding="utf-8")
        return self.index_path

    def write_source(self, relative_path: str) -> Path:
        path = self.root / "src" / relative_path
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text("namespace Test;\n", encoding="utf-8")
        return path

    def run_with_patched_paths(self, action: Callable[[], list[str]]) -> list[str]:
        with (
            patch.object(check_snippet_inventory, "ROOT", self.root),
            patch.object(check_snippet_inventory, "DOCS_ROOT", self.docs_root),
            patch.object(check_snippet_inventory, "INDEX_PATH", self.index_path),
            patch.object(check_snippet_inventory, "SOURCE_ROOTS", self.source_roots),
        ):
            return action()


if __name__ == "__main__":
    unittest.main()
