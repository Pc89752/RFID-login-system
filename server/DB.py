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

def insert_touples_into_DB(table_name,attributes):
    # conn = sqlite3.connect('DB.db')
    cursor = conn.cursor()

    # 將元組資料插入資料庫
    try:
        placeholder = ",".join(["?" for _ in attributes[0]])
        query = f"INSERT INTO {table_name} VALUES ({placeholder})"
        cursor.executemany(query, attributes)
        conn.commit()
        print("成功插入元組到資料庫")
    except Exception as e:
        print("插入元組失敗:", str(e))
    
    # 關閉連線
# def check_inner_code(code,studentID):
#     cursor = conn.cursor()
    
#     # 假設資料表名稱為 "your_table"
#     table_name = 'InnerCode'
#     # 查詢資料庫中是否存在對應的資料
#     try:
#         query = f"SELECT * FROM {table_name} WHERE Code = {code} AND StudentID = '{studentID}'"
#         cursor.execute(query)
#     except Exception as e:
#         pass

#     # 檢查是否存在結果
#     result = cursor.fetchone()

#     # 關閉連線
   

#     # 根據是否存在結果回傳 True 或 False
#     if result is not None:
#         return True
#     else:
#         return False
# def check_account(student_ID, password):
#     cursor = conn.cursor()

#     # 假設資料表名稱為 "your_table"
#     table_name = 'StudentAccount'

#     # 查詢資料庫中是否存在對應的資料
#     try:
#         query = f"SELECT * FROM {table_name} WHERE StudentID = ? AND Password = ?"
#         cursor.execute(query, (student_ID, password))   
#     except Exception as e:
#         pass

#     # 檢查是否存在結果
#     result = cursor.fetchone()

#     # 關閉連線
   

#     # 根據是否存在結果回傳 True 或 False
#     if result is not None:
#         return True
#     else:
#         return False

def get_tuple(table_name,pk):
    cursor = conn.cursor()

    # 查詢資料庫中是否存在對應的資料
    try:
        pk_col = find_pk_col(table_name)
        query = f"SELECT * FROM {table_name} WHERE {pk_col} = '{pk}'"
        cursor.execute(query)
        # cursor.execute(query, pk) 
        result = cursor.fetchone()
        return result  
    except Exception as e:
        print("尋找失敗:", str(e))
        return None

    # 檢查是否存在結果
def update_DB(table, pk, tuple_data):
    cursor = conn.cursor()
    pk_col = find_pk_col(table)
    # 執行更新操作
    try:
        set_columns = ", ".join(f"{column[0]}='{column[1]}'" for column in tuple_data)
        query = f"UPDATE {table} SET {set_columns} WHERE {pk_col}='{pk}'"
        cursor.execute(query)
        conn.commit()
        print("資料更新成功")
    except Exception as e:
        print("資料更新失敗:", str(e))

def find_pk_col(table):
    cursor = conn.cursor()

    # 查詢資料庫中是否存在對應的資料
    try:
        cursor.execute(f"pragma table_info('{table}')")
        infos = cursor.fetchall()
        pk_col = ""
        for info in infos:
            if info[5] == 1:
                pk_col = info[1]
        return pk_col
    except Exception as e:
        print("尋找失敗:", str(e))
        return None

if __name__ == "__main__":
    import json
    student_json_path  = os.path.join(parent, "student_account.json")
    computer_usage_path = os.path.join(parent, "computer_usage.json")
    innerCode_path = os.path.join(parent, "Inner_code.json")
    # 重新建立table
    deleteTable(student_json_path)
    deleteTable(computer_usage_path)
    deleteTable(innerCode_path)
    create_table(student_json_path)
    create_table(computer_usage_path)
    create_table(innerCode_path)
    insertTable(student_json_path)
    insertTable(computer_usage_path)
    insertTable(innerCode_path)
    
    
    insert_touples_into_DB("ComputerUsage",[[12356,12345,12358,456789]])
    print(get_tuple("StudentAccount","U10916001"))
    update_DB("StudentAccount","U10916001",[("Password","23456")])  
    
    conn.close()
    # jsonData = json.load(open(os.path.join(parent, "student_account.json"), "r"))
    # cmmdStr = f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES "
    # for row in jsonData["rows"]:
    #     row = [f"'{e}'" for e in row]
    #     cmmdStr += "(" + ",".join(row) + "),"
    # cmmdStr = cmmdStr[0:-1]
    # print(cmmdStr)
    # # print(f"INSERT INTO {jsonData['table_name']} ({','.join([e[0] for e in jsonData['cols']])}) VALUES ({'),('.join([','.join(e) for e in jsonData['rows']])})")