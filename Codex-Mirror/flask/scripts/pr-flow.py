#!/usr/bin/env python3
import subprocess, sys, yaml, os

cfg = yaml.safe_load(open("config/flask.yaml"))
def sh(*a):
    print("+", " ".join(a))
    subprocess.check_call(a)

def main(msg="ritual: forming step"):
    branch = cfg["git"]["branch"]
    remote = cfg["git"]["remote"]
    sh("git","init","-q")
    sh("git","checkout","-B",branch)
    sh("git","add","castings","telemetry/outbox","engrams/opal",".gitignore","README.md","README-QUICKSTART.md")
    try:
        sh("git","commit","-m",msg)
    except subprocess.CalledProcessError:
        pass
    # reset origin
    try:
        remotes = subprocess.check_output(["git","remote"]).decode().split()
        if "origin" in remotes:
            sh("git","remote","remove","origin")
    except Exception:
        pass
    sh("git","remote","add","origin",remote)
    try:
        sh("git","push","-u","origin",branch)
    except subprocess.CalledProcessError:
        print("Push failed. Ensure credentials and remote permissions.")

if __name__=="__main__":
    main(" ".join(sys.argv[1:]) if len(sys.argv)>1 else "ritual: forming step")
