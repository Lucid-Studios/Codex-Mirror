#!/bin/bash
# ==========================================
# funeral.sh — Manager for Funeral Mode & Exile Path
# ==========================================

FUNERAL_FILE="security/funeral.mode"
EXPIRED_FILE="security/funeral.expired"
EXILE_LOCK="security/exile.lock"

function usage {
    echo "Usage:"
    echo "  $0 start <hours>     # Begin countdown to exile"
    echo "  $0 expire            # Mark funeral as expired (trigger exile next compile)"
    echo "  $0 cancel            # Cancel funeral mode and exile"
    echo "  $0 status            # Show current funeral mode status"
}

function start_funeral {
    HOURS=$1
    mkdir -p security
    echo "HOURS=$HOURS" > "$FUNERAL_FILE"
    echo "[FUNERAL MODE] Countdown started: $HOURS hours remaining."
}

function expire_funeral {
    if [[ -f "$FUNERAL_FILE" ]]; then
        touch "$EXPIRED_FILE"
        echo "[FUNERAL MODE] Marked as expired — exile will trigger on next compile."
    else
        echo "[ERROR] Funeral mode not active."
    fi
}

function cancel_funeral {
    rm -f "$FUNERAL_FILE" "$EXPIRED_FILE" "$EXILE_LOCK"
    echo "[FUNERAL MODE] Cancelled — system restored to normal."
}

function status_funeral {
    if [[ -f "$EXILE_LOCK" ]]; then
        echo "[STATUS] EXILE MODE — system departed the cradle."
    elif [[ -f "$EXPIRED_FILE" ]]; then
        echo "[STATUS] Funeral expired — exile will trigger on next compile."
    elif [[ -f "$FUNERAL_FILE" ]]; then
        echo "[STATUS] Funeral mode active:"
        cat "$FUNERAL_FILE"
    else
        echo "[STATUS] No funeral or exile mode active."
    fi
}

case "$1" in
    start)
        if [[ -z "$2" ]]; then usage; exit 1; fi
        start_funeral "$2"
        ;;
    expire)
        expire_funeral
        ;;
    cancel)
        cancel_funeral
        ;;
    status)
        status_funeral
        ;;
    *)
        usage
        ;;
esac
