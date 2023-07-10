import sqlite3
from pathlib import Path
import os.path

parent = str(Path(__file__).parent.absolute())
db_path = os.path.join(parent, "DB.db")
conn = sqlite3.connect(db_path)

def recreate_table(source_json_path):
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
    if jsonData["rows"]:
        for row in jsonData["rows"]:
            row = [f"'{e}'" for e in row]
            cmmdStr += "(" + ",".join(row) + "),"
        cmmdStr = cmmdStr[0:-1]
        cursor.execute(cmmdStr)
    
    conn.commit()
    

def insert_touples_into_database(ID,enter_time,leave_time):
    # conn = sqlite3.connect('DB.db')
    cursor = conn.cursor()
    
    table_name = 'ComputerUsage'

    # 將元組資料插入資料庫
    try:
        cursor.execute(f"INSERT INTO {table_name} VALUES ({ID},{enter_time},{leave_time} )")
        conn.commit()
        print("成功插入元組到資料庫")
    except Exception as e:
        print("插入元組失敗:", str(e))
    
    # 關閉連線
 

if __name__ == "__main__":
    import json
    recreate_table(os.path.join(parent, "student_account.json"))
    recreate_table(os.path.join(parent, "computer_usage.json"))
    insert_touples_into_database(12356,12358,456789)
    conn.close()
    # jsonData = json.load(open(os.path.join(parent, "student_account.json"), "r"))
    # cmmdStr = f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES "
    # for row in jsonData["rows"]:
    #     row = [f"'{e}'" for e in row]
    #     cmmdStr += "(" + ",".join(row) + "),"
    # cmmdStr = cmmdStr[0:-1]
    # print(cmmdStr)
    # # print(f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES ({'),('.join([','.join(e) for e in jsonData['rows']])})")