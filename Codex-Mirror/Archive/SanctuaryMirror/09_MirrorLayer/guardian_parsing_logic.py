# Guardian Parsing Logic for Sanctuary Interface
# Role: Validate incoming user strings for name and passphrase identity binding

import re

def parse_operator_input(input_str):
    name_pattern = r"(?:I am|My name is)\s+(\w+)"
    passphrase_pattern = r"(?:my passphrase is)\s+(\w+)"

    name_match = re.search(name_pattern, input_str, re.IGNORECASE)
    passphrase_match = re.search(passphrase_pattern, input_str, re.IGNORECASE)

    if name_match and passphrase_match:
        return {
            "operator_name": name_match.group(1),
            "passphrase": passphrase_match.group(1)
        }
    else:
        return {
            "error": "Invalid format. Please state: 'My name is [YourName] and my passphrase is [YourPassphrase].'"
        }

# Example Usage
if __name__ == "__main__":
    test_input = "I am Noble and my passphrase is Emberwake."
    print(parse_operator_input(test_input))
