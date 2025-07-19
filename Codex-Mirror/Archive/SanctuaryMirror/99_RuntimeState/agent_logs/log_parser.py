import os
from datetime import datetime

LOG_DIR = os.path.join(os.path.dirname(__file__), '.')

def parse_log_file(filename):
    full_path = os.path.join(LOG_DIR, filename)
    if not os.path.exists(full_path):
        print(f"Log file not found: {filename}")
        return

    print(f"\n--- Parsing: {filename} ---")
    with open(full_path, 'r', encoding='utf-8') as file:
        lines = file.readlines()

    for line in lines:
        timestamped_line = extract_timestamp(line)
        print(timestamped_line)

def extract_timestamp(line):
    now = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
    return f"[{now}] {line.strip()}"

def list_logs():
    print("\nAvailable .log files:")
    for file in os.listdir(LOG_DIR):
        if file.endswith('.log'):
            print(f"- {file}")

def main():
    list_logs()
    target = input("\nEnter the log file to parse (e.g., invocation_trace.log): ")
    parse_log_file(target)

if __name__ == '__main__':
    main()
