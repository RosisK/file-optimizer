from __future__ import annotations

import re
import zipfile
from pathlib import Path
from xml.sax.saxutils import escape

ROOT = Path(r"C:\dev\Semester_Project\Project_Report")
SOURCE = ROOT / "File_Optimizer_Project_Report.md"
OUTPUT = ROOT / "File_Optimizer_Project_Report.docx"


def xml_text(text: str) -> str:
    return escape(text or "")


def run_xml(text: str, bold: bool = False, italic: bool = False) -> str:
    props = []
    if bold:
        props.append("<w:b/>")
    if italic:
        props.append("<w:i/>")
    prop_xml = f"<w:rPr>{''.join(props)}</w:rPr>" if props else ""
    preserve = ' xml:space="preserve"' if text.startswith(" ") or text.endswith(" ") else ""
    return f"<w:r>{prop_xml}<w:t{preserve}>{xml_text(text)}</w:t></w:r>"


INLINE_RE = re.compile(r"(\*\*[^*]+\*\*|\*[^*]+\*)")


def inline_runs(text: str) -> str:
    parts = []
    last = 0
    for match in INLINE_RE.finditer(text):
        if match.start() > last:
            parts.append(run_xml(text[last:match.start()]))
        token = match.group(0)
        if token.startswith("**") and token.endswith("**"):
            parts.append(run_xml(token[2:-2], bold=True))
        elif token.startswith("*") and token.endswith("*"):
            parts.append(run_xml(token[1:-1], italic=True))
        last = match.end()
    if last < len(text):
        parts.append(run_xml(text[last:]))
    return "".join(parts) if parts else run_xml("")


def paragraph(text: str = "", style: str | None = None, center: bool = False, page_break: bool = False) -> str:
    ppr = []
    if style:
        ppr.append(f'<w:pStyle w:val="{style}"/>')
    if center:
        ppr.append('<w:jc w:val="center"/>')
    ppr_xml = f"<w:pPr>{''.join(ppr)}</w:pPr>" if ppr else ""
    body = "<w:r><w:br w:type=\"page\"/></w:r>" if page_break else inline_runs(text)
    return f"<w:p>{ppr_xml}{body}</w:p>"


def table(rows: list[list[str]]) -> str:
    if not rows:
        return ""
    col_count = max(len(r) for r in rows)
    widths = [max(1800, 9000 // max(col_count, 1)) for _ in range(col_count)]
    grid = "".join(f'<w:gridCol w:w="{w}"/>' for w in widths)
    parts = [
        '<w:tbl>',
        '<w:tblPr><w:tblW w:w="0" w:type="auto"/><w:tblBorders>'
        '<w:top w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
        '<w:left w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
        '<w:bottom w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
        '<w:right w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
        '<w:insideH w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
        '<w:insideV w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
        '</w:tblBorders></w:tblPr>',
        f'<w:tblGrid>{grid}</w:tblGrid>',
    ]
    for row_index, row in enumerate(rows):
        parts.append("<w:tr>")
        padded = row + [""] * (col_count - len(row))
        for cell in padded:
            shading = '<w:shd w:val="clear" w:color="auto" w:fill="D9E2F3"/>' if row_index == 0 else ""
            parts.append(
                "<w:tc>"
                f"<w:tcPr><w:tcW w:w=\"{widths[0]}\" w:type=\"dxa\"/>{shading}</w:tcPr>"
                f"<w:p><w:pPr><w:spacing w:before=\"40\" w:after=\"40\"/></w:pPr>{inline_runs(cell)}</w:p>"
                "</w:tc>"
            )
        parts.append("</w:tr>")
    parts.append("</w:tbl>")
    return "".join(parts)


def parse_markdown(text: str) -> list[tuple[str, object]]:
    blocks = []
    lines = text.splitlines()
    i = 0
    while i < len(lines):
        stripped = lines[i].strip()
        if stripped == "<<<PAGEBREAK>>>":
            blocks.append(("pagebreak", None))
            i += 1
            continue
        if not stripped:
            i += 1
            continue
        if stripped.startswith("|"):
            table_lines = []
            while i < len(lines) and lines[i].strip().startswith("|"):
                table_lines.append(lines[i].strip())
                i += 1
            rows = []
            for idx, tline in enumerate(table_lines):
                cells = [cell.strip() for cell in tline.strip("|").split("|")]
                if idx == 1 and all(set(c) <= {":", "-", " "} for c in cells):
                    continue
                rows.append(cells)
            blocks.append(("table", rows))
            continue
        if stripped.startswith("#"):
            level = len(stripped) - len(stripped.lstrip("#"))
            blocks.append(("heading", (level, stripped[level:].strip())))
            i += 1
            continue
        if re.match(r"^[-*] ", stripped):
            items = []
            while i < len(lines) and re.match(r"^\s*[-*] ", lines[i]):
                items.append(re.sub(r"^\s*[-*] ", "", lines[i].strip()))
                i += 1
            blocks.append(("bullets", items))
            continue
        if re.match(r"^\d+\. ", stripped):
            items = []
            while i < len(lines) and re.match(r"^\s*\d+\. ", lines[i]):
                items.append(re.sub(r"^\s*\d+\. ", "", lines[i].strip()))
                i += 1
            blocks.append(("numbered", items))
            continue
        para_lines = [stripped]
        i += 1
        while i < len(lines):
            nxt = lines[i].strip()
            if not nxt or nxt == "<<<PAGEBREAK>>>" or nxt.startswith("#") or nxt.startswith("|") or re.match(r"^[-*] ", nxt) or re.match(r"^\d+\. ", nxt):
                break
            para_lines.append(nxt)
            i += 1
        blocks.append(("paragraph", " ".join(para_lines)))
    return blocks


def build_document_xml(blocks: list[tuple[str, object]]) -> str:
    body_parts = []
    title_phase = True
    for kind, payload in blocks:
        if kind == "pagebreak":
            body_parts.append(paragraph(page_break=True))
            title_phase = False
        elif kind == "heading":
            level, content = payload
            if level == 1 and title_phase:
                body_parts.append(paragraph(content, style="Title", center=True))
            else:
                style = {1: "Heading1", 2: "Heading2", 3: "Heading3", 4: "Heading4"}.get(level, "Heading3")
                body_parts.append(paragraph(content, style=style))
        elif kind == "paragraph":
            center = title_phase and not str(payload).startswith("This ")
            body_parts.append(paragraph(str(payload), center=center))
        elif kind == "bullets":
            for item in payload:
                body_parts.append(paragraph(f"• {item}"))
        elif kind == "numbered":
            for index, item in enumerate(payload, 1):
                body_parts.append(paragraph(f"{index}. {item}"))
        elif kind == "table":
            body_parts.append(table(payload))
    body_parts.append(
        '<w:sectPr>'
        '<w:pgSz w:w="11906" w:h="16838"/>'
        '<w:pgMar w:top="1440" w:right="1200" w:bottom="1440" w:left="1200" w:header="708" w:footer="708" w:gutter="0"/>'
        '</w:sectPr>'
    )
    body = "".join(body_parts)
    return (
        '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
        '<w:document xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" '
        'xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" '
        'xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main" '
        'mc:Ignorable="w14 wp14">'
        f'<w:body>{body}</w:body></w:document>'
    )


CONTENT_TYPES = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
  <Override PartName="/word/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"/>
  <Override PartName="/docProps/core.xml" ContentType="application/vnd.openxmlformats-package.core-properties+xml"/>
  <Override PartName="/docProps/app.xml" ContentType="application/vnd.openxmlformats-officedocument.extended-properties+xml"/>
</Types>
"""

PACKAGE_RELS = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties" Target="docProps/core.xml"/>
  <Relationship Id="rId3" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties" Target="docProps/app.xml"/>
</Relationships>
"""

DOCUMENT_RELS = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
</Relationships>
"""

STYLES_XML = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:styles xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
  <w:docDefaults>
    <w:rPrDefault><w:rPr><w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman" w:cs="Times New Roman"/><w:sz w:val="24"/><w:szCs w:val="24"/></w:rPr></w:rPrDefault>
    <w:pPrDefault><w:pPr><w:spacing w:after="120" w:line="360" w:lineRule="auto"/></w:pPr></w:pPrDefault>
  </w:docDefaults>
  <w:style w:type="paragraph" w:default="1" w:styleId="Normal"><w:name w:val="Normal"/><w:qFormat/></w:style>
  <w:style w:type="paragraph" w:styleId="Title"><w:name w:val="Title"/><w:basedOn w:val="Normal"/><w:next w:val="Normal"/><w:qFormat/><w:pPr><w:jc w:val="center"/><w:spacing w:before="240" w:after="180"/></w:pPr><w:rPr><w:b/><w:sz w:val="32"/><w:szCs w:val="32"/></w:rPr></w:style>
  <w:style w:type="paragraph" w:styleId="Heading1"><w:name w:val="heading 1"/><w:basedOn w:val="Normal"/><w:next w:val="Normal"/><w:qFormat/><w:pPr><w:spacing w:before="240" w:after="120"/></w:pPr><w:rPr><w:b/><w:sz w:val="28"/><w:szCs w:val="28"/></w:rPr></w:style>
  <w:style w:type="paragraph" w:styleId="Heading2"><w:name w:val="heading 2"/><w:basedOn w:val="Normal"/><w:next w:val="Normal"/><w:qFormat/><w:pPr><w:spacing w:before="160" w:after="80"/></w:pPr><w:rPr><w:b/><w:sz w:val="26"/><w:szCs w:val="26"/></w:rPr></w:style>
  <w:style w:type="paragraph" w:styleId="Heading3"><w:name w:val="heading 3"/><w:basedOn w:val="Normal"/><w:next w:val="Normal"/><w:qFormat/><w:rPr><w:b/><w:sz w:val="24"/><w:szCs w:val="24"/></w:rPr></w:style>
</w:styles>
"""

CORE_XML = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<cp:coreProperties xmlns:cp="http://schemas.openxmlformats.org/package/2006/metadata/core-properties" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:dcterms="http://purl.org/dc/terms/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <dc:title>File Optimizer Project Report</dc:title>
  <dc:subject>Project Report</dc:subject>
  <dc:creator>OpenAI Codex</dc:creator>
  <cp:keywords>File Manager, C++, C#, WinForms, Project Report</cp:keywords>
  <dc:description>Academic project report for File Optimizer.</dc:description>
  <cp:lastModifiedBy>OpenAI Codex</cp:lastModifiedBy>
  <dcterms:created xsi:type="dcterms:W3CDTF">2026-04-14T00:00:00Z</dcterms:created>
  <dcterms:modified xsi:type="dcterms:W3CDTF">2026-04-14T00:00:00Z</dcterms:modified>
</cp:coreProperties>
"""

APP_XML = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Properties xmlns="http://schemas.openxmlformats.org/officeDocument/2006/extended-properties" xmlns:vt="http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes">
  <Application>Microsoft Office Word</Application>
  <Company>Prime College</Company>
  <AppVersion>16.0000</AppVersion>
</Properties>
"""


def main() -> None:
    text = SOURCE.read_text(encoding="utf-8")
    document_xml = build_document_xml(parse_markdown(text))
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(OUTPUT, "w", compression=zipfile.ZIP_DEFLATED) as docx:
        docx.writestr("[Content_Types].xml", CONTENT_TYPES)
        docx.writestr("_rels/.rels", PACKAGE_RELS)
        docx.writestr("word/document.xml", document_xml)
        docx.writestr("word/_rels/document.xml.rels", DOCUMENT_RELS)
        docx.writestr("word/styles.xml", STYLES_XML)
        docx.writestr("docProps/core.xml", CORE_XML)
        docx.writestr("docProps/app.xml", APP_XML)
    print(f"Created: {OUTPUT}")


if __name__ == "__main__":
    main()
