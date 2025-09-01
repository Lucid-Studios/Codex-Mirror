# Prime Security Patch v1 — How to wire it

## 1) Add these files to your project
- `security/prime_guard.tex`
- `security/secure_input.tex`
- `ritual/ritual_threads.tex`

## 2) Load them early (in your main.tex or core preamble)
```tex
\input{security/prime_guard.tex}
\input{security/secure_input.tex}
\input{ritual/ritual_threads.tex}
```

## 3) Replace fragile calls
- `\input{threads.map}` → `\SafeInput{threads.map}`
- remove any `\immediate\write18{...}` / `\openin` + `\read` and use:
```tex
\IfFileExists{security/COVENANT.key}{%
  \GenerateThreadsMap{ritual/keys/chapter2keys}{threads.map}%
}{%
  \GenerateThreadsMap{.}{threads.map}%
}
\SafeInput{threads.map}
```

## 4) Guard logs and effects
- Wrap `\typeout` debugging as `\DebugTypeout{...}`
- Wrap visible dev-only elements as `\DevOnly{...}`
- Keep `\PRIMEtrue` for release; flip to `\PRIMEfalse` during local debug.

## 5) Security posture
- No shell-escape, no terminal reads.
- Deterministic builds in PRIME (random seeded).
- Lua sandbox blocks `os.execute`, `io.popen`, `package.loadlib`, and denies absolute paths or `..` traversal.
- No secrets in TeX; use presence-only keys (`COVENANT.key`).

## 6) Overleaf hygiene
- Keep folders that can be empty by adding a `.keep` or `README.md` file.
- Avoid non-ASCII/fancy chars in any filename that you `\input` or `\includegraphics`.