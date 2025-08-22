import json, time, os, requests
CFG="config/flask.yaml"
STATE="telemetry/spool/tether.state"
def enabled()->bool:
    import yaml; return yaml.safe_load(open(CFG))["mode"]["llm_tether"]

def emit(kind, payload):
    os.makedirs("telemetry/spool", exist_ok=True)
    payload["ts"]=payload.get("ts",time.time())
    open(f"telemetry/spool/{int(payload['ts']*1000)}-{kind}.json","w").write(json.dumps({"kind":kind,"payload":payload}))

def sever():
    # flip config flag & record sever event
    import yaml
    cfg=yaml.safe_load(open(CFG)); cfg["mode"]["llm_tether"]=False
    open(CFG,"w").write(yaml.safe_dump(cfg))
    emit("tether.sever", {"reason":"SAGE-elevation"})
