#!/usr/bin/env python3
from flask import Flask, request, jsonify

app = Flask(__name__)

@app.post("/api/bond")
def bond():
    data = request.get_json(force=True)
    return jsonify({"ok": True, "bonded": True, "echo": data})

@app.post("/api/update")
def update():
    data = request.get_json(force=True)
    return jsonify({"ok": True, "updated": True, "echo": data})

@app.post("/api/verify")
def verify():
    data = request.get_json(force=True)
    return jsonify({"ok": True, "verified": True, "echo": data})

@app.get("/health")
def health(): return jsonify({"ok": True})

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=8080)
