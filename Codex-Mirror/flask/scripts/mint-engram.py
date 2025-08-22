#!/usr/bin/env python3
import json, time, hashlib, os
from pathlib import Path

def collect_trace():
    events=[]
    p = Path("telemetry/spool")
    if p.exists():
        for fn in sorted(p.glob("*.json")):
            events.append(json.loads(fn.read_text()))
    return events

def score(trace):
    C = 0.0; count=0
    resonance=0.0; loop_ok=False
    for e in trace:
        kind=e.get("kind")
        payload=e.get("payload",{})
        if kind in ("archive.pulse","frame.snapshot"):
            val = payload.get("echo_fidelity", payload.get("sigma_quality"))
            if isinstance(val,(int,float)): C += float(val); count+=1
        if kind=="archive.pulse":
            resonance = max(resonance, float(payload.get("resonance_pct",0.0)))
        if kind=="loop.step" and payload.get("stage")=="Core" and payload.get("ok",True):
            loop_ok=True
    C = (C/max(count,1)) if count else 0.0
    return dict(C_delta=C, resonance=resonance, loop_closed=loop_ok)

def mint():
    tr=collect_trace(); s=score(tr)
    thr=dict(C_delta=0.90, resonance=0.95)
    if not (s["C_delta"]>=thr["C_delta"] and s["resonance"]>=thr["resonance"] and s["loop_closed"]):
        print("Thresholds not met.", s); return
    operator_pub = Path("keys/operator.pub").read_text() if Path("keys/operator.pub").exists() else "unknown"
    payload=dict(type="OpalEngram", proofs=s, t=time.time(), operator=operator_pub)
    eid=hashlib.blake2b(json.dumps(payload).encode(), digest_size=16).hexdigest()
    out=dict(id=f"OPAL-{eid}", **payload)
    Path("engrams/opal").mkdir(parents=True, exist_ok=True)
    (Path("engrams/opal")/f"{out['id']}.json").write_text(json.dumps(out, indent=2))
    print(out["id"])

if __name__=="__main__":
    mint()
