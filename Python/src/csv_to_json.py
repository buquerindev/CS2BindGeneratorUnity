import csv
import json
import os

# Path with the CSV and JSON
path = "G:\\Programacion\\CS2BindGeneratorUnity\\PersonalResources\\JSON\\"

json_data_path = path + "commands.json"

bool_csv_path = path + "bool.csv"
enum_csv_path = path + "enum.csv"
float_csv_path = path + "float.csv"
int_csv_path = path + "int.csv"
string_csv_path = path + "string.csv"

# Remove the JSON file if it exists
if os.path.exists(json_data_path):
    os.remove(json_data_path)

# Initialize the JSON data structure
json_data = {
    "commands": []
}

# Helper function to load a CSV and add a type field
def load_csv_with_type(path, type_name):
    with open(path, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f, delimiter=';')
        for row in reader:
            row["type"] = type_name
            json_data["commands"].append(row)

# Load each CSV file with its corresponding type
load_csv_with_type(bool_csv_path, "bool")
load_csv_with_type(enum_csv_path, "enum")
load_csv_with_type(float_csv_path, "float")
load_csv_with_type(int_csv_path, "int")
load_csv_with_type(string_csv_path, "string")

# Create a JSON file from the CSV data
with open(json_data_path, mode='w', encoding='utf-8') as json_file:
    json.dump(json_data, json_file, ensure_ascii=False, indent=4)

