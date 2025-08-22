import json, os, sys, hashlib, hmac, base64, time
from pathlib import Path

GEMS = Path("golden/chapter-08-philosophers-stone/gems")
KEYS = Path("keys"); KEYS.mkdir(exist_ok=True, parents=True)
MANTLE_SALT = b"MotherMantle|v1"

def fold_gems():
    h = hashlib.blake2b(digest_size=32)
    for p in sorted(GEMS.glob("**/*")):
        if p.is_file():
            h.update(hashlib.blake2b(p.read_bytes(), digest_size=32).digest())
            h.update(p.name.encode())
    return h.digest()

def derive_key():
    seed = fold_gems()
    # “first interlock” — HKDF-lite w/ mantle salt; swap out with real HKDF later
    k = hmac.new(MANTLE_SALT, seed, hashlib.sha256).digest()
    return base64.urlsafe_b64encode(k).decode()

def main():
    if not any(GEMS.glob("**/*")):
        print("No gems found in chapter 8. Aborting."); sys.exit(2)
    key = derive_key()
    Path("keys/operator.key").write_text(key)
    Path("keys/operator.pub").write_text(hashlib.sha256(key.encode()).hexdigest())
    print("Operator key derived & sealed.")
if __name__ == "__main__":
    main()
