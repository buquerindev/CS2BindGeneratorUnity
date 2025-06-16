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

# Read each CSV file and append its data to the JSON structure
with open(bool_csv_path, newline="", encoding="utf-8") as f:
    reader = csv.DictReader(f, delimiter=';')
    for row in reader:
        json_data["commands"].append(row)

with open(enum_csv_path, newline="", encoding="utf-8") as f:
    reader = csv.DictReader(f, delimiter=';')
    for row in reader:
        json_data["commands"].append(row)

with open(float_csv_path, newline="", encoding="utf-8") as f:
    reader = csv.DictReader(f, delimiter=';')
    for row in reader:
        json_data["commands"].append(row)

with open(int_csv_path, newline="", encoding="utf-8") as f:
    reader = csv.DictReader(f, delimiter=';')
    for row in reader:
        json_data["commands"].append(row)

with open(string_csv_path, newline="", encoding="utf-8") as f:
    reader = csv.DictReader(f, delimiter=';')
    for row in reader:
        json_data["commands"].append(row)

# Create a JSON file from the CSV data
with open(json_data_path, mode='w', encoding='utf-8') as json_file:
    json.dump(json_data, json_file, ensure_ascii=False, indent=4)

