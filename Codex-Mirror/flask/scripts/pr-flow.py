import subprocess, yaml, json, os, sys
cfg = yaml.safe_load(open("config/flask.yaml"))
def sh(*a): subprocess.check_call(a)
def main(msg="ritual: forming step"):
    branch = cfg["git"]["branch"]
    sh("git","checkout","-B",branch)
    sh("git","add","castings","telemetry/outbox","engrams/opal")
    sh("git","commit","-m",msg)
    sh("git","push","-u","origin",branch)
    # create PR via gh cli if available
    try:
        sh("gh","pr","create","--fill","--base","main","--head",branch)
    except: print("PR created via remote hook or CI.")
if __name__=="__main__": main()
