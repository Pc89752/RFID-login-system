from flask import Flask, request, render_template
import json
from DB import DB
from pathlib import Path
import os.path
from datetime import datetime

app = Flask(__name__)
# TODO: Refresh DEV_TOKEN once a while
DEV_TOKEN = r"a!Qe*1!H/jgr8clv6yDQjaadi{I}C0B7Viwd}W1TsI=-f)lMWyHrm9V}Kx,2IF\1"
PARDIR = str(Path(__file__).parent.absolute())
COMPUTER_USAGE_DB_PATH = os.path.join(PARDIR, "ComputerUsage.db")
INFO_DB_PATH = os.path.join(PARDIR, "StudentINFO.db")
SUCCESS_CODE = [0, 5]

@app.route('/submit/<action>', methods=['POST'])
def submit(action):
    jsonData = json.loads(request.get_data(as_text=True))
    computer_usage_db = DB(COMPUTER_USAGE_DB_PATH)
    info_db = DB(INFO_DB_PATH)

    status_code = None

    # response
    if action == 'innerCode_login':
        status_code, usageRecordID = handle_innerCode_login(computer_usage_db, info_db, jsonData)
    elif action == 'account_login':
        status_code, usageRecordID = handle_account_login(computer_usage_db, info_db, jsonData)
    elif action == 'devPass':
        status_code, usageRecordID = handle_devPass(computer_usage_db, jsonData)
    else:
        return
    
    if status_code == None:
        return
    
    if status_code in SUCCESS_CODE:
        return {"status_code": 0, "usageRecordID": usageRecordID}
    return {"status_code": status_code}

@app.route('/closeReport', methods=['POST'])
def handle_close_report():
    jsonData = json.loads(request.get_data(as_text=True))
    computer_usage_db = DB(COMPUTER_USAGE_DB_PATH)
    usageRecordID =  jsonData["usageRecordID"]
    now = datetime.now()
    leaveTime = now.strftime("%Y-%m-%d, %H:%M:%S")
    computer_usage_db.update_DB("records", usageRecordID, [("leaveTime", leaveTime)])
    result = computer_usage_db.getTuple("records", usageRecordID)
    computer_usage_db.delete_tuple("ComputerUsage", result[2])
    return {}

def handle_account_login(computer_usage_db, info_db, jsonData):
    # get username and password
    cID, sID, password = jsonData["computerID"], jsonData["account"], jsonData["password"]
    # Check account and password
    valid_user, valid_password = False, False
    t = info_db.getTuple("StudentAccount", sID)
    check = computer_usage_db.getTuple("ComputerUsage",sID)
    # print(check)
    if check:
        return 6,None

    if t:
        valid_user = True
        if password == t[1]:
            valid_password = True
        
    if not valid_user:
        return 1, None
    if not valid_password:
        return 2, None
    
    usageRecordID = computer_usage_db.rowCount("records")
    now = datetime.now()
    loginTime = now.strftime("%Y-%m-%d, %H:%M:%S")
    computer_usage_db.insertTuples("ComputerUsage", [[sID]])
    computer_usage_db.insertTuples("records",[[usageRecordID, cID, sID, loginTime, "null"]])
    return 0, usageRecordID

def handle_innerCode_login(computer_usage_db, info_db, jsonData):
    # get username and password
    innerCode, cID = jsonData["innerCode"], jsonData["computerID"]
    
    # check innerCode and studentID
    t = info_db.getTuple("InnerCode", innerCode)
    if not t:
        return 3, None
    
    sID = t[1]
    check = computer_usage_db.getTuple("ComputerUsage",sID)
    if check:
        return 6,None

    usageRecordID = computer_usage_db.rowCount("records")
    now = datetime.now()
    loginTime = now.strftime("%Y-%m-%d, %H:%M:%S")
    computer_usage_db.insertTuples("ComputerUsage", [[sID]])
    computer_usage_db.insertTuples("records",[[usageRecordID, cID, sID, loginTime, "null"]])
    return 0, usageRecordID
    

def handle_devPass(computer_usage_db, jsonData):
    inp, cID = jsonData["DEV_TOKEN"], jsonData["computerID"]
    usageRecordID = computer_usage_db.rowCount("records")
    now = datetime.now()
    loginTime = now.strftime("%Y-%m-%d, %H:%M:%S")
    computer_usage_db.insertTuples("records", [[usageRecordID, cID, "DEV", loginTime, "null"]])
    if DEV_TOKEN != inp:
        return 4, None
    return 5, usageRecordID

#註冊學生的學號和卡號
@app.route('/register', methods=['POST'])
def register_post():
  info_db = DB(INFO_DB_PATH)
  # 從表單資料中獲取學號和卡號
  student_id = request.form['student_id']
  ic_card_id = request.form['ic_card_id']
  print(student_id, ic_card_id)
  if info_db.find_touple("InnerCode",ic_card_id):
    return {"success": False}
  
  info_db.insertTuples("InnerCode", [[ic_card_id,student_id]])
  return {"success": True, "student_id": student_id}

@app.route('/register', methods=['GET'])
def register_get():
    return render_template('registerWeb.html')

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)