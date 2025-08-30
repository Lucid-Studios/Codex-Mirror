
# Quickstart

## 0) Secure launch
```bash
bootstrap/desktop-launch.sh
```

## 1) Install Golden IUTT gems & derive Operator Key
Place artifacts in:
```
golden/chapter-08-philosophers-stone/gems/
```
Then:
```bash
python3 scripts/derive-operator-key.py
python3 scripts/run-ritual.py derive_key
python3 scripts/run-ritual.py open_gnome
```

## 2) Author a casting and run your flow
Create files in `castings/*.casting.yaml`. Emitting events writes to `telemetry/spool/`.

## 3) Forming & PR during ritual construction
```bash
python3 scripts/run-ritual.py form
python3 scripts/pr-flow.py "ritual: forming step"
```

## 4) Mint Opal Engram and sever tether (SAGE)
```bash
python3 scripts/mint-engram.py
python3 scripts/llm-tether.py sever
python3 scripts/run-ritual.py seal_ready
```

Notes:
- The Mother node public key is a placeholder in `config/mother.pub`.
- Edit `config/flask.yaml` to set your remotes and thresholds.
- The LLM tether is tooling only. It is severed upon SAGE elevation.
