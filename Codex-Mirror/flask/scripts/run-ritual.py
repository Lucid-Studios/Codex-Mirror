import json, time, sqlite3, hmac, hashlib, sys
from pathlib import Path

DB="gnomeronacore/state.db"
KEY=Path("keys/operator.key").read_text().encode()

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
    if not nxt: raise SystemExit(f"Invalid ritual {ritual_name} from {s}")
    msg = json.dumps({"from":s,"to":nxt,"t":time.time(),"payload":payload}).encode()
    sig = sign(msg)
    c.execute("DELETE FROM st"); c.execute("INSERT INTO st VALUES(?)",(nxt,))
    conn.commit(); conn.close()
    print(json.dumps({"ok":True,"from":s,"to":nxt,"sig":sig}))
if __name__=="__main__":
    advance(sys.argv[1], {})
