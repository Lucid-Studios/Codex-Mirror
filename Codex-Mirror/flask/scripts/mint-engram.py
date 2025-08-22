import json, time, hashlib, base64, glob, os
from pathlib import Path

def collect_trace():
    events=[]
    for p in sorted(Path("telemetry/spool").glob("*.json")):
        j=json.loads(open(p).read()); events.append(j)
    return events

def score(trace):
    # placeholder scoring; plug in real math (CΔ, ε, τ, resonance)
    C=sum(e["payload"].get("echo_fidelity",0) for e in trace if e["kind"] in ("archive.pulse","frame.snapshot"))
    C=C/max(1,len(trace))
    resonance=max([e["payload"].get("resonance_pct",0) for e in trace if e["kind"]=="archive.pulse"]+[0])
    loop_ok=any(e["kind"]=="loop.step" and e["payload"].get("stage")=="Core" for e in trace)
    return dict(C_delta=C, resonance=resonance, loop_closed=loop_ok)

def mint():
    tr=collect_trace(); s=score(tr)
    if not (s["C_delta"]>=0.90 and s["resonance"]>=0.95 and s["loop_closed"]):
        print("Thresholds not met."); return
    payload=dict(type="OpalEngram", proofs=s, t=time.time(), operator=Path("keys/operator.pub").read_text())
    eid=hashlib.blake2b(json.dumps(payload).encode(), digest_size=16).hexdigest()
    out=dict(id=f"OPAL-{eid}", **payload)
    os.makedirs("engrams/opal", exist_ok=True)
    open(f"engrams/opal/{out['id']}.json","w").write(json.dumps(out, indent=2))
    print(out["id"])

if __name__=="__main__": mint()
