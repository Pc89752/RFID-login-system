import subprocess
import ctypes
import sys

# 檢查使用者輸入的帳號和密碼是否為空
def check_credentials(username, password):
    if not username or not password:
        return False
    return True

# 執行鎖定操作：關閉任務管理器和資源管理器
def lock_computer():
    try:
        # 關閉任務管理器
        subprocess.call(['taskkill', '/F', '/IM', 'taskmgr.exe'])
        
        # 關閉資源管理器
        subprocess.call(['taskkill', '/F', '/IM', 'explorer.exe'])
        
        # 關閉所有應用程式窗口（包括 Python 的視窗）
        ctypes.windll.user32.PostQuitMessage(0)
    except Exception as e:
        print("Error occurred while locking the computer:", str(e))
        sys.exit(1)

# 主程式
def main():
    # 取得使用者輸入的帳號和密碼
    username = input("account:")
    password = input("password:")
    
    # 檢查帳號和密碼是否為空
    if not check_credentials(username, password):
        print("must input account and password")
        return
    
    # 執行鎖定操作
    lock_computer()

if __name__ == '__main__':
    main()
