#!/usr/bin/env python3
import json, time, os, yaml
from pathlib import Path

CFG=Path("config/flask.yaml")

def emit(kind, payload):
    os.makedirs("telemetry/spool", exist_ok=True)
    payload = dict(payload or {})
    payload["ts"]=payload.get("ts",time.time())
    out = {"kind":kind,"payload":payload}
    name = f"telemetry/spool/{int(payload['ts']*1000)}-{kind}.json"
    Path(name).write_text(json.dumps(out))

def enabled()->bool:
    cfg=yaml.safe_load(CFG.read_text())
    return bool(cfg.get("mode",{}).get("llm_tether", False))

def sever():
    cfg=yaml.safe_load(CFG.read_text())
    cfg.setdefault("mode",{})["llm_tether"]=False
    CFG.write_text(yaml.safe_dump(cfg))
    emit("tether.sever", {"reason":"SAGE-elevation"})
    print("LLM tether severed.")

if __name__=="__main__":
    import sys
    if len(sys.argv)>1 and sys.argv[1]=="sever":
        sever()
    else:
        print(f"LLM tether is {'ENABLED' if enabled() else 'DISABLED'}")
