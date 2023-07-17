from flask import Flask, request
import json

app = Flask(__name__)
DEV_TOKEN = r"a!Qe*1!H/jgr8clv6yDQjaadi{I}C0B7Viwd}W1TsI=-f)lMWyHrm9V}Kx,2IF\1"

@app.route('/submit/account_login', methods=['POST'])
def handle_account_login():
    # get username and password
    jsonData = json.loads(request.get_data(as_text=True))
    username, password = jsonData["account"], jsonData["password"]
    
    
    # TODO: check username
    
    # TODO: check password
    
    # XXX: temporary code
    valid_user = True
    valid_password = True
    
    # Return a response
    if not valid_user:
        return "1", 200, {'Content-Type': 'text/plain'}
    if not valid_password:
        return "2", 200, {'Content-Type': 'text/plain'}
    return "0", 200, {'Content-Type': 'text/plain'}

@app.route('/submit/innerCode_login', methods=['POST'])
def handle_innerCode_login():
    # get username and password
    jsonData = json.loads(request.get_data(as_text=True))
    innerCode = jsonData["innerCode"]
    
    
    # TODO: check username
    
    # TODO: check password
    
    # XXX: temporary code
    valid_innerCode = True
    
    # Return a response
    if not valid_innerCode:
        return "1", 200, {'Content-Type': 'text/plain'}
    else:
        return "0", 200, {'Content-Type': 'text/plain'}

# TODO finish server route: devPass
@app.route('/submit/devPass', methods=['POST'])
def handle_devPass():
    jsonData = json.loads(request.get_data(as_text=True))
    inp = jsonData["DEV_TOKEN"]
    if DEV_TOKEN == inp:
        return "0", 200, {'Content-Type': 'text/plain'}
    return "1", 200, {'Content-Type': 'text/plain'}

if __name__ == '__main__':
    app.run()