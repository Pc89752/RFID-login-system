from flask import Flask, request
import json

app = Flask(__name__)

@app.route('/submit', methods=['POST'])
def handle_submit():
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

if __name__ == '__main__':
    app.run()