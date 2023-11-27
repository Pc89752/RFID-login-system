from flask import Flask, request
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

@app.route('/submit/<action>', methods=['POST'])
def submit(action):
    jsonData = json.loads(request.get_data(as_text=True))
    record_db = DB(COMPUTER_USAGE_DB_PATH)
    info_db = DB(INFO_DB_PATH)

    status_code = None

    # response
    if action == 'innerCode_login':
        status_code, usageRecordID = handle_innerCode_login(record_db, info_db, jsonData)
    elif action == 'account_login':
        status_code, usageRecordID = handle_account_login(record_db, info_db, jsonData)
    elif action == 'devPass':
        status_code, usageRecordID = handle_devPass(record_db, jsonData)
    else:
        return
    
    if status_code == None:
        return
    
    if status_code != 0:
        return {"status_code": status_code}
    
    return {"status_code": 0, "usageRecordID": usageRecordID}

@app.route('/closeReport', methods=['POST'])
def handle_close_report():
    jsonData = json.loads(request.get_data(as_text=True))
    record_db = DB(COMPUTER_USAGE_DB_PATH)
    
    usageRecordID =  jsonData["usageRecordID"]
    now = datetime.now()
    leaveTime = now.strftime("%Y-%m-%d, %H:%M:%S")
    record_db.update_DB("records", usageRecordID, [("leaveTime", leaveTime)])
    result = record_db.getTuple("records", usageRecordID)
    # FIXME: get studentID from the DB
    record_db.delete_tuple("ComputerUsage", result[2])
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
    sID = None
    t = info_db.getTuple("Code", innerCode)
    check = info_db.getTuple("InnerCode",sID)
    if not check:
        if t:
            valid_innerCode = True
            sID = t[1]
        
        # Return a response
        if not valid_innerCode or not sID:
            return 3, None
        
        usageRecordID = computer_usage_db.rowCount("ComputerUsage")
        now = datetime.now()
        loginTime = now.strftime("%Y-%m-%d, %H:%M:%S")
        computer_usage_db.insertTuples("ComputerUsage", [[sID]])
        computer_usage_db.insertTuples("records",[[usageRecordID, cID, sID, loginTime, "null"]])
        return 0, usageRecordID
    return 6,None

def handle_devPass(record_db, jsonData):
    inp, cID = jsonData["DEV_TOKEN"], jsonData["computerID"]
    if DEV_TOKEN != inp:
        return 4, None
    return 5,None

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)