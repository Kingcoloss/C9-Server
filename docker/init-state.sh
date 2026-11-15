#!/bin/sh
# One-shot preparation of the shared state folder (runs as root in the c9-init service).
#   portal/  operator key, audit log, parameter overlays, cookie keys      (uid 1654)
#   trace/   packet trace JSONL per service and day                         (uid 1654)
#   lobby/   character store of the lobby service                           (uid 1654)
#   secrets/ agent.token (1654), grafana/admin-password.txt (472), db/password.txt (70, group 1654)
# Bind mounts on Docker Desktop for Windows ignore chown/chmod; errors there are expected and ignored.
set -eu
STATE=/c9/state
APP=1654
mkdir -p "$STATE/portal" "$STATE/trace" "$STATE/lobby" "$STATE/secrets/grafana" "$STATE/secrets/db"
random() { tr -dc 'A-Za-z0-9' </dev/urandom | head -c 32; }
umask 077
[ -s "$STATE/secrets/grafana/admin-password.txt" ] || random >"$STATE/secrets/grafana/admin-password.txt"
[ -s "$STATE/secrets/db/password.txt" ] || random >"$STATE/secrets/db/password.txt"
own() { chown "$@" 2>/dev/null || true; }
mode() { chmod "$@" 2>/dev/null || true; }
own "$APP:$APP" "$STATE"
own -R "$APP:$APP" "$STATE/portal" "$STATE/trace" "$STATE/lobby"
own "$APP:$APP" "$STATE/secrets"
mode 0711 "$STATE/secrets"
[ -e "$STATE/secrets/agent.token" ] && own "$APP:$APP" "$STATE/secrets/agent.token" && mode 0600 "$STATE/secrets/agent.token"
own -R 472:0 "$STATE/secrets/grafana"
mode 0500 "$STATE/secrets/grafana"
mode 0400 "$STATE/secrets/grafana/admin-password.txt"
own -R "70:$APP" "$STATE/secrets/db"
mode 0550 "$STATE/secrets/db"
mode 0440 "$STATE/secrets/db/password.txt"
echo "c9-init: state ready in $STATE"
