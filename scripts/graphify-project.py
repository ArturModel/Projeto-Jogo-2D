"""Local Graphify lifecycle shared by PowerShell and Bash; no LLM/backend calls."""
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys

ROOT = Path(__file__).resolve().parent.parent
VERSION = "0.9.96"
os.chdir(ROOT)
os.environ.setdefault("PYTHONIOENCODING", "utf-8")
os.environ.setdefault("GRAPHIFY_NO_TIPS", "1")

def command(name):
    found = shutil.which(name)
    if found:
        return found
    suffix = ".exe" if os.name == "nt" else ""
    for folder in [Path.home()/".local/bin", Path.home()/".santarita/bin"]:
        candidate = folder/(name+suffix)
        if candidate.is_file():
            return str(candidate)
    raise RuntimeError(f"{name} unavailable; install with uv tool install graphifyy=={VERSION}")

def run(*args, quiet=False):
    result = subprocess.run(args, check=True, text=True, encoding="utf-8", errors="replace",
                            stdout=subprocess.PIPE if quiet else None)
    return result.stdout or ""

def sources():
    return sorted(ROOT.glob("Projeto Jogo 2D/Assets/Scripts/**/*.cs"))+sorted(ROOT.glob("Projeto Jogo 2D/Assets/Editor/**/*.cs"))

def fingerprint():
    return {p.relative_to(ROOT).as_posix():hashlib.sha256(p.read_bytes()).hexdigest() for p in sources()}

def validate():
    graph_path=ROOT/"graphify-out/graph.json"
    graph=json.loads(graph_path.read_text(encoding="utf-8"))
    nodes=graph.get("nodes",[])
    if not nodes:
        raise RuntimeError("Graph is empty")
    indexed=set()
    for node in nodes:
        name=node.get("source_file")
        if not name:
            continue
        path=Path(name)
        normalized=name.replace("\\","/")
        if path.is_absolute() or (len(normalized)>1 and normalized[1]==":"):
            raise RuntimeError(f"Non-portable graph source: {name}")
        target=(ROOT/path).resolve()
        if not target.is_relative_to(ROOT) or not target.is_file():
            raise RuntimeError(f"Invalid graph source: {name}")
        if target.suffix.lower() in {".png",".wav",".mp4",".json",".env",".pem",".key"}:
            raise RuntimeError(f"Forbidden source: {name}")
        indexed.add(normalized)
    expected=fingerprint()
    missing=set(expected)-indexed
    if missing:
        raise RuntimeError("C# sources absent from graph: "+", ".join(sorted(missing)))
    scan=json.loads((ROOT/"graphify-out/scan-state.json").read_text(encoding="utf-8"))
    if scan["csharp_sha256"]!=expected:
        raise RuntimeError("C# files changed since refresh")
    canvas=json.loads((ROOT/"docs/architecture-vault/generated/graph.canvas").read_text(encoding="utf-8"))
    if not canvas.get("nodes"):
        raise RuntimeError("Empty Obsidian canvas")
    for rel in ["graphify-out/GRAPH_REPORT.md","graphify-out/graph.html",
                "docs/architecture-vault/manual/Visao_Geral_Arquitetura.md",
                "docs/ai-memory/CURRENT_STATE.md"]:
        if not (ROOT/rel).is_file() or not (ROOT/rel).stat().st_size:
            raise RuntimeError(f"Missing output: {rel}")
    for symbol in ["GameState","Capitulo2Portal","BochaSimulation","SantaRitaShallowStairs"]:
        run(command("graphify"),"query",symbol,"--budget","120",quiet=True)
    print(f"PASS: {len(expected)} C# sources covered; portable paths; current hashes; canvas/HTML/report; four real queries.")
    print("Scope: structural index only. No gameplay, art, audio or campaign approval inferred.")

def main():
    mode=sys.argv[1] if len(sys.argv)>1 else "validate"
    if mode=="setup":
        try:
            cli=command("graphify")
        except RuntimeError:
            run(command("uv"),"tool","install",f"graphifyy=={VERSION}")
            cli=command("graphify")
        print(run(cli,"--version",quiet=True).strip())
        print("Ready. No agent hooks or project rules were overwritten. Open docs/architecture-vault in Obsidian.")
    elif mode=="refresh":
        cli=command("graphify")
        version=run(cli,"--version",quiet=True).strip()
        if version!=f"graphify {VERSION}":
            raise RuntimeError(f"Expected graphify {VERSION}, found {version}; review upgrades explicitly")
        before=fingerprint()
        manual={p:p.read_bytes() for p in (ROOT/"docs/architecture-vault/manual").glob("*.md")}
        run(cli,"update",".")
        run(cli,"export","obsidian","--dir","docs/architecture-vault/generated")
        run(cli,"export","html")
        if before!=fingerprint():
            raise RuntimeError("Source changed while indexing; run refresh again")
        if any(p.read_bytes()!=data for p,data in manual.items()):
            raise RuntimeError("Manual note changed during export")
        state={"graphify_version":VERSION,"scope":"local AST; C# coverage verified; no semantic document extraction", "csharp_sha256":before}
        (ROOT/"graphify-out/scan-state.json").write_text(json.dumps(state,indent=2,ensure_ascii=False)+"\n",encoding="utf-8")
        validate()
    elif mode=="validate":
        validate()
    else:
        raise RuntimeError("Usage: graphify-project.py setup|refresh|validate")

if __name__=="__main__":
    try:
        main()
    except (RuntimeError, OSError, ValueError, subprocess.CalledProcessError) as exc:
        print(f"FAIL: {exc}",file=sys.stderr)
        sys.exit(1)
