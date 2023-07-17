import sqlite3
from pathlib import Path
import os.path
import json

parent = str(Path(__file__).parent.absolute())
db_path = os.path.join(parent, "DB.db")
conn = sqlite3.connect(db_path)

def create_table(source_json_path):
    cursor = conn.cursor()
    jsonData = json.load(open(source_json_path, "r"))
    if not jsonData:
        print(f"{source_json_path}: wrong format!")
        return
    cursor.execute(f"CREATE TABLE {jsonData['table_name']} ({','.join([' '.join(e) for e in jsonData['cols']])}, PRIMARY KEY ({jsonData['cols'][0][0]}))")
    conn.commit()
  
def deleteTable(source_json_path):
    jsonData = json.load(open(source_json_path, "r"))
    if not jsonData:
        print(f"{source_json_path}: wrong format!")
        return
    
    cursor = conn.cursor()
    try:
        cursor.execute(f"DROP TABLE {jsonData['table_name']}")
    except:
        pass
    
    conn.commit()

def insertTable(source_json_path):
    cursor = conn.cursor()
    jsonData = json.load(open(source_json_path, "r"))
    
    if not jsonData:
        print(f"{source_json_path}: wrong format!")
        return
    cmmdStr = f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES "
    
    if jsonData["rows"]:
        for row in jsonData["rows"]:
            row = [f"'{e}'" for e in row]
            cmmdStr += "(" + ",".join(row) + "),"
        cmmdStr = cmmdStr[0:-1]
        cursor.execute(cmmdStr)
    
    conn.commit()

def insert_touples_into_computer_usage(ComputerID,StudentID,enter_time,leave_time):
    # conn = sqlite3.connect('DB.db')
    cursor = conn.cursor()
    
    table_name = 'ComputerUsage'

    # 將元組資料插入資料庫
    try:
        cursor.execute(f"INSERT INTO {table_name} VALUES ({ComputerID},{StudentID},{enter_time},{leave_time} )")
        conn.commit()
        print("成功插入元組到資料庫")
    except Exception as e:
        print("插入元組失敗:", str(e))
    
    # 關閉連線

def check_account(student_ID, password):
    cursor = conn.cursor()

    # 假設資料表名稱為 "your_table"
    table_name = 'StudentAccount'

    # 查詢資料庫中是否存在對應的資料
    try:
        query = f"SELECT * FROM {table_name} WHERE StudentID = ? AND Password = ?"
        cursor.execute(query, (student_ID, password))   
    except Exception as e:
        print("尋找失敗:", str(e))

    # 檢查是否存在結果
    result = cursor.fetchone()

    # 關閉連線
    conn.close()

    # 根據是否存在結果回傳 True 或 False
    if result is not None:
        return True
    else:
        return False

    

if __name__ == "__main__":
    import json
    student_json_path  = os.path.join(parent, "student_account.json")
    computer_usage_path = os.path.join(parent, "computer_usage.json")
    internal_code_path = os.path.join(parent, "Internal_code.json")
    # 重新建立table
    deleteTable(student_json_path)
    deleteTable(computer_usage_path)
    deleteTable(internal_code_path)
    create_table(student_json_path)
    create_table(computer_usage_path)
    create_table(internal_code_path)
    insertTable(student_json_path)
    insertTable(computer_usage_path)
    insertTable(internal_code_path)
    
    
    insert_touples_into_computer_usage(12356,12345,12358,456789)
    insert_touples_into_computer_usage(12456,12345,12358,456789)
    
    if(check_account("U10916001", "2345")):
        print("存在這筆資料")
    else:
        print("不存在這筆資料")
    conn.close()
    # jsonData = json.load(open(os.path.join(parent, "student_account.json"), "r"))
    # cmmdStr = f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES "
    # for row in jsonData["rows"]:
    #     row = [f"'{e}'" for e in row]
    #     cmmdStr += "(" + ",".join(row) + "),"
    # cmmdStr = cmmdStr[0:-1]
    # print(cmmdStr)
    # # print(f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES ({'),('.join([','.join(e) for e in jsonData['rows']])})")