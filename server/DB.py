import sqlite3
import json

class DB():
    def __init__(self, db_path) -> None:
        self.conn = sqlite3.connect(db_path)

    def createTableFromJson(self, source_json_path):
        cursor = self.conn.cursor()
        jsonData = json.load(open(source_json_path, "r"))
        if not jsonData:
            print(f"{source_json_path}: wrong format!")
            return
        cursor.execute(f"CREATE TABLE {jsonData['table_name']} ({','.join([' '.join(e) for e in jsonData['cols']])}, PRIMARY KEY ({jsonData['cols'][0][0]}))")
        self.conn.commit()
    
    def dropTable(self, source_json_path):
        jsonData = json.load(open(source_json_path, "r"))
        if not jsonData:
            print(f"{source_json_path}: wrong format!")
            return
        
        cursor = self.conn.cursor()
        try:
            cursor.execute(f"DROP TABLE {jsonData['table_name']}")
        except:
            pass
        
        self.conn.commit()

    def insertTableFromJson(self, source_json_path):
        cursor = self.conn.cursor()
        jsonData = json.load(open(source_json_path, "r"))
        
        if not jsonData:
            print(f"{source_json_path}: wrong format!")
            return
        table_name, tuples = jsonData["table_name"], jsonData["rows"]

        self.insertTuples(table_name, tuples)

    def insertTuples(self, table_name, attributes):
        if not attributes:
            return

        # 將元組資料插入資料庫
        cursor = self.conn.cursor()
        try:
            placeholder = ",".join(["?" for _ in attributes[0]])
            query = f"INSERT INTO {table_name} VALUES ({placeholder})"
            cursor.executemany(query, attributes)
            self.conn.commit()
        except Exception as e:
            print(e.with_traceback())

    def rowCount(self, table_name):
        cursor = self.conn.cursor()
        return cursor.execute(f"SELECT COUNT(*) FROM {table_name}").fetchone()[0]
    
# 關閉連線
# def check_inner_code(code,studentID):
#     cursor = self.conn.cursor()
    
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
#     cursor = self.conn.cursor()

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

    def getTuple(self, table_name, pk):

        # 查詢資料庫中是否存在對應的資料
        cursor = self.conn.cursor()
        try:
            pk_col = self.find_pk_col(table_name)
            query = f"SELECT * FROM {table_name} WHERE {pk_col} = '{pk}'"
            cursor.execute(query)
            # cursor.execute(query, pk) 
            result = cursor.fetchone()
            return result  
        except Exception as e:
            print(e.with_traceback())
            return None

    def update_DB(self, table, pk, tuple_data):
        cursor = self.conn.cursor()
        pk_col = self.find_pk_col(table)
        # 執行更新操作
        try:
            set_columns = ", ".join(f"{column[0]}='{column[1]}'" for column in tuple_data)
            query = f"UPDATE {table} SET {set_columns} WHERE {pk_col}='{pk}'"
            cursor.execute(query)
            self.conn.commit()
        except Exception as e:
            print(e.with_traceback())

    def find_pk_col(self, table):
        cursor = self.conn.cursor()

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
            print(e.with_traceback())
            return None
        
def reCreateTableFromJson(db, json_path):
    db.dropTable(json_path)
    db.createTableFromJson(json_path)
    db.insertTableFromJson(json_path)

if __name__ == "__main__":
    from pathlib import Path
    import os.path
    parent = str(Path(__file__).parent.absolute())
    account_json_path  = os.path.join(parent, "student_account.json")
    computer_usage_path = os.path.join(parent, "computer_usage.json")
    innerCode_path = os.path.join(parent, "Inner_code.json")
    
    # Build "StudentINFO.db"
    info_db_path = os.path.join(parent, "StudentINFO.db")
    info_db = DB(info_db_path)
    reCreateTableFromJson(info_db, account_json_path)
    reCreateTableFromJson(info_db, innerCode_path)
    
    # Build "ComputerUsage.db"
    usage_db_path = os.path.join(parent, "ComputerUsage.db")
    usage_db = DB(usage_db_path)
    reCreateTableFromJson(usage_db, computer_usage_path)
    
    
    # insertTuples("ComputerUsage",[[12356, 12345, 12358, 456789, 1]])
    # print(get_tuple("StudentAccount","U10916001"))
    # update_DB("StudentAccount","U10916001",[("Password","23456")])