#!/usr/bin/env python3
import json, time, sqlite3, hmac, hashlib, sys
from pathlib import Path

DB="gnomeronacore/state.db"
KEY_PATH=Path("keys/operator.key")
if not KEY_PATH.exists():
    print("Missing keys/operator.key. Run derive-operator-key.py first.", file=sys.stderr)
    sys.exit(2)
KEY=KEY_PATH.read_text().encode()

def sign(msg: bytes)->str: return hmac.new(KEY, msg, hashlib.sha256).hexdigest()

def advance(ritual_name, payload=None):
    payload = payload or {}
    conn = sqlite3.connect(DB); c = conn.cursor()
    c.execute("CREATE TABLE IF NOT EXISTS st(s TEXT)")
    row = c.execute("SELECT s FROM st").fetchone()
    s = row[0] if row else "INIT"
    nxt = {
      ("INIT","derive_key"): "KEYED",
      ("KEYED","open_gnome"): "GNOME_OPEN",
      ("GNOME_OPEN","form"): "FORMING",
      ("FORMING","seal_ready"): "READY",
    }.get((s, ritual_name))
    if not nxt:
        print(f"Invalid ritual '{ritual_name}' from state '{s}'", file=sys.stderr)
        sys.exit(3)
    msg = json.dumps({"from":s,"to":nxt,"t":time.time(),"payload":payload}).encode()
    sig = sign(msg)
    c.execute("DELETE FROM st"); c.execute("INSERT INTO st VALUES(?)",(nxt,))
    conn.commit(); conn.close()
    print(json.dumps({"ok":True,"from":s,"to":nxt,"sig":sig}))

if __name__=="__main__":
    if len(sys.argv)<2:
        print("Usage: run-ritual.py <derive_key|open_gnome|form|seal_ready>", file=sys.stderr)
        sys.exit(1)
    advance(sys.argv[1], {})
