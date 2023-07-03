import sqlite3
from pathlib import Path
import os.path

parent = str(Path(__file__).parent.absolute())
db_path = os.path.join(parent, "DB.db")
conn = sqlite3.connect(db_path)

def recreate_student_table(source_json_path):
    import json
    
    jsonData = json.load(open(source_json_path, "r"))
    if not jsonData:
        print(f"{source_json_path}: wrong format!")
        return
    
    cursor = conn.cursor()
    try:
        cursor.execute(f"DROP TABLE {jsonData['table_name']}")
    except:
        pass
    
    cursor.execute(f"CREATE TABLE {jsonData['table_name']} ({','.join([' '.join(e) for e in jsonData['cols']])})")
    cmmdStr = f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES "
    for row in jsonData["rows"]:
        row = [f"'{e}'" for e in row]
        cmmdStr += "(" + ",".join(row) + "),"
    cmmdStr = cmmdStr[0:-1]
    cursor.execute(cmmdStr)
    
    conn.commit()
    conn.close()

if __name__ == "__main__":
    import json
    recreate_student_table(os.path.join(parent, "student_account.json"))
    # jsonData = json.load(open(os.path.join(parent, "student_account.json"), "r"))
    # cmmdStr = f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES "
    # for row in jsonData["rows"]:
    #     row = [f"'{e}'" for e in row]
    #     cmmdStr += "(" + ",".join(row) + "),"
    # cmmdStr = cmmdStr[0:-1]
    # print(cmmdStr)
    # # print(f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES ({'),('.join([','.join(e) for e in jsonData['rows']])})")