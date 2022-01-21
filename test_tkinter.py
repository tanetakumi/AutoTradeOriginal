import threading
import tkinter as tk
from highlow import Browser
import time

class Application(tk.Frame):
    
    def __init__(self, master = None):
        super().__init__(master)

        self.master.title("ボタンの作成")       # ウィンドウタイトル
        self.master.geometry("300x100")         # ウィンドウサイズ(幅x高さ)

        #--------------------------------------------------------
        # ボタンの作成
        button = tk.Button(self.master, 
                           text = "ボタン",                 # ボタンの表示名
                           command = self.button_click      # クリックされたときに呼ばれるメソッド
                           )
        button.pack(side='left')
        # ボタンの作成
        button2 = tk.Button(self.master, 
                           text = "ボタン2",                 # ボタンの表示名
                           command = self.button_click2      # クリックされたときに呼ばれるメソッド
                           )
        button2.pack(side='left')
        
        # エントリーの作成
        self.entry_inp = tk.StringVar()
        entry = tk.Entry(self.master,
            width = 30,                     # ウィジェットの幅（文字数で指定）
            justify = tk.LEFT,              # tk.RIGHT:右寄せ、tk.LEFT:左寄せ、tk.CENTER:中央寄せ
            textvariable = self.entry_inp   # 表示する値
            )
        entry.pack()

        # チェックボックスの作成
        self.checked = tk.BooleanVar()
        chk = tk.Checkbutton(self.master,
            text='Pythonを使用する',
            variable=self.checked
            )
        chk.place(x=50, y=70)
        #--------------------------------------------------------
    def button_click(self):
        print(self.entry_inp.get())
        print(self.checked.get())
    
    def button_click2(self):
        th = threading.Thread(target=self.operate)
        th.start()
    
    def operate(self):
        self.browser = Browser()
        self.browser.open_first_page()


    
if __name__ == "__main__":
    root = tk.Tk()
    app = Application(master = root)
    app.mainloop()