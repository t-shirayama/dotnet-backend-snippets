from __future__ import annotations

import tempfile
import unittest
from pathlib import Path

import check_markdown_links


class GitHubAnchorTests(unittest.TestCase):
    def test_github_anchor_keeps_japanese_fragment(self) -> None:
        self.assertEqual("実務逆引き", check_markdown_links.github_anchor("実務逆引き"))

    def test_github_anchor_removes_ascii_and_fullwidth_punctuation(self) -> None:
        self.assertEqual(
            "openapi-api仕様の基本",
            check_markdown_links.github_anchor("OpenAPI / API（仕様）の基本"),
        )

    def test_anchors_for_adds_suffix_to_duplicate_headings(self) -> None:
        with tempfile.TemporaryDirectory() as temporary_directory:
            markdown = Path(temporary_directory) / "README.md"
            markdown.write_text(
                "# 見出し\n\n## 実務逆引き\n\n## 実務逆引き\n",
                encoding="utf-8",
            )

            anchors = check_markdown_links.anchors_for(markdown)

        self.assertIn("見出し", anchors)
        self.assertIn("実務逆引き", anchors)
        self.assertIn("実務逆引き-1", anchors)


if __name__ == "__main__":
    unittest.main()
