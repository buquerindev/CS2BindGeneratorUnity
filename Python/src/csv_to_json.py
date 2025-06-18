import csv
import json
import os

# Path with the CSV and JSON
path = "G:\\Programacion\\CS2BindGeneratorUnity\\PersonalResources\\JSON\\"

json_data_path = path + "commands.json"
binds_data_path = path + "binds.json"

bool_csv_path = path + "bool.csv"
enum_csv_path = path + "enum.csv"
float_csv_path = path + "float.csv"
int_csv_path = path + "int.csv"
string_csv_path = path + "string.csv"

binds_csv_path = path + "binds.csv"

# # Remove the JSON file if it exists
# if os.path.exists(json_data_path):
#     os.remove(json_data_path)


# Initialize the JSON data structure
commands_json_data = {
    "commands": []
}

binds_json_data = {
    "binds": []
}



# Helper function to load a CSV and add a type field
def load_csv_with_type(path, type_name):
    with open(path, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f, delimiter=';')
        for row in reader:
            row["type"] = type_name
            commands_json_data["commands"].append(row)

def load_csv(path):
    with open(path, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f, delimiter=';')
        for row in reader:
            binds_json_data["binds"].append(row)

# Load each CSV file with its corresponding type
load_csv_with_type(bool_csv_path, "bool")
load_csv_with_type(enum_csv_path, "enum")
load_csv_with_type(float_csv_path, "float")
load_csv_with_type(int_csv_path, "int")
load_csv_with_type(string_csv_path, "string")
load_csv(binds_csv_path)

# Create a JSON file from the CSV data for commands
with open(json_data_path, mode='w', encoding='utf-8') as commands_json_file:
    json.dump(commands_json_data, commands_json_file, ensure_ascii=False, indent=4)

commands_json_file.close()

# Create a JSON file from the CSV data for binds
with open(binds_data_path, mode='w', encoding='utf-8') as binds_json_file:
    json.dump(binds_json_data, binds_json_file, ensure_ascii=False, indent = 4)

binds_json_file.close()
