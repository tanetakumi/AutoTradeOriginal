from locale import currency
import time
from typing import Literal
from selenium import webdriver
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome import service as fs
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.options import Options
import re

# coding: UTF-8
from bs4 import BeautifulSoup


class IndexPageObject:
    # Seleniumで探す要素をクラス定数として定義する
    Turbo       = (By.ID, 'ChangingStrikeOOD0')
    HighLow     = (By.ID, 'ChangingStrike0')
    TurboSp     = (By.ID, 'FixedPayoutHLOOD0')
    HighLowSp   = (By.ID, 'FixedPayoutHL0')
    Period = (By.XPATH, '//*[@id="App_mainContainer__2JivZ"]/div[1]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div')
    Currency = (By.XPATH, '//*[@id="App_mainContainer__2JivZ"]/div[1]/div[1]/div[1]/div[1]/div[4]/div[3]/div[1]/div')
    CurrencyInput = (By.XPATH, '//*[@id="App_mainContainer__2JivZ"]/div[1]/div[1]/div[1]/div[1]/div[4]/div[3]/div[2]/div/div[1]/div/input')

    HighEntry = (By.XPATH, '//*[@id="TradePanel_oneClickHighButton__3OAFf"]/div/div[2]')

    OneClick = (By.XPATH, '//*[@id="App_mainContainer__2JivZ"]/div[1]/div[2]/div/div[2]/div/div[2]/div[1]/div')

    Input   = (By.CLASS_NAME, 'MoneyInputField_amount__6JeTs')
    CLASS1  = (By.CLASS_NAME, 'post1')
    CLASS2  = (By.CLASS_NAME, 'post2')
    CLASS3  = (By.CLASS_NAME, 'post3')  # これは存在しない要素
    CSS     = (By.CSS_SELECTOR, '.post1')
    XPATH   = (By.XPATH, '//li[@class="post1"]')

class Browser:
    def __init__(self,headress = False) -> None:
        # create options
        options = Options()
        options.add_experimental_option('excludeSwitches', ['enable-logging'])
        options.use_chromium = True
        if headress:
            options.add_argument('--headless')

        # create driver services
        chrome_service = fs.Service(ChromeDriverManager().install())

        # driver
        self.driver = webdriver.Chrome(service = chrome_service, options=options)


    def open_first_page(self):
        print("open first page")
        self.driver.get("https://app.highlow.com/quick-demo")

    def click_element(self):
        # print("click")

        self.driver.find_element(*IndexPageObject.Period).click()
        self.driver.find_element(By.ID, '30000').click()
        time.sleep(0.5)
        self.driver.find_element(*IndexPageObject.Currency).click()
        self.driver.find_element(*IndexPageObject.CurrencyInput).send_keys('EURUSD')
        self.driver.find_element(By.ID, 'EUR/USD').click()
        time.sleep(0.5)
        self.driver.find_element(*IndexPageObject.HighEntry).click()

    def enable_oneclick(self):
        oneclick_element = self.driver.find_element(*IndexPageObject.OneClick)
        data_test_value = oneclick_element.get_attribute('data-test')
        if re.search('Enable',data_test_value):
            print("すでにワンクリック注文は有効でした。")
        else:
            oneclick_element.click()
            print("ワンクリック注文を有効化しました。")


    def investment(self, period : str , direction : str , currency : str):
        select = self.get_period_number(period)
        if select["tab"] == -1:
            print("適切な引数が渡されませんでした。")
            print(period)
            return False
        
        
        

        
    def get_period_number(self, period : str):
        res = {"tab" : -1, "period" : -1, "sml" : -1 }
        
        if re.search('(turbo|ターボ)', period, flags=re.IGNORECASE):

            if re.search('(スプ|sp)', period, flags=re.IGNORECASE):
                if re.search('(30|３０)(s|秒)', period, flags=re.IGNORECASE):
                    res = {"tab" : IndexPageObject.TurboSp, "period" : 0, "sml" : -1 }
                elif re.search('(1|１)(m|分)', period, flags=re.IGNORECASE):
                    res = {"tab" : 2, "period" : 1, "sml" : -1 }
                elif re.search('(3|３)(m|分)', period, flags=re.IGNORECASE):
                    res = {"tab" : 2, "period" : 2, "sml" : -1 }
                elif re.search('(5|５)(m|分)', period, flags=re.IGNORECASE):
                    res = {"tab" : 2, "period" : 3, "sml" : -1 }

            else:
                if re.search('(30|３０)(s|秒)', period, flags=re.IGNORECASE):
                    res = {"tab" : 0, "period" : 0, "sml" : -1 }
                elif re.search('(1|１)(m|分)', period, flags=re.IGNORECASE):
                    res = {"tab" : 0, "period" : 1, "sml" : -1 }
                elif re.search('(3|３)(m|分)', period, flags=re.IGNORECASE):
                    res = {"tab" : 0, "period" : 2, "sml" : -1 }
                elif re.search('(5|５)(m|分)', period, flags=re.IGNORECASE):
                    res = {"tab" : 0, "period" : 3, "sml" : -1 }

        elif re.search('highlow', period, flags=re.IGNORECASE):
            # highlow spread
            if re.search('(スプ|sp)', period, flags=re.IGNORECASE):

                if re.search('(15|１５)(m|分)', period, flags=re.IGNORECASE):
                    if re.search('sho|短', period, flags=re.IGNORECASE):
                        res = {"tab" : 3, "period" : 0, "sml" : 0 }
                    elif re.search('mid|中', period, flags=re.IGNORECASE):
                        res = {"tab" : 3, "period" : 0, "sml" : 1 }
                    elif re.search('lon|長', period, flags=re.IGNORECASE):
                        res = {"tab" : 3, "period" : 0, "sml" : 2 }

                elif re.search('(1|１)(h|時)', period, flags=re.IGNORECASE):
                    res = {"tab" : 3, "period" : 1, "sml" : -1 }
                elif re.search('(1|１)(d|日)', period, flags=re.IGNORECASE):
                    res = {"tab" : 3, "period" : 2, "sml" : -1 }
            else:
                if re.search('(15|１５)(m|分)', period, flags=re.IGNORECASE):
                    if re.search('sho|短', period, flags=re.IGNORECASE):
                        res = {"tab" : 1, "period" : 0, "sml" : 0 }
                    elif re.search('mid|中', period, flags=re.IGNORECASE):
                        res = {"tab" : 1, "period" : 0, "sml" : 1 }
                    elif re.search('lon|長', period, flags=re.IGNORECASE):
                        res = {"tab" : 1, "period" : 0, "sml" : 2 }
                elif re.search('(1|１)(h|時)', period, flags=re.IGNORECASE):
                    res = {"tab" : 1, "period" : 1, "sml" : -1 }
                elif re.search('(1|１)(d|日)', period, flags=re.IGNORECASE):
                    res = {"tab" : 1, "period" : 1, "sml" : -1 }               
        
        return res
        

def javascript():
    options = Options()

    options.add_experimental_option('excludeSwitches', ['enable-logging'])
    #これがエラーをなくすコードです。ブラウザ制御コメントを非表示化しています

    options.use_chromium = True
    #これがエラーをなくすコードです。WebDriverのテスト動作をTrueにしています

    # ヘッドレスモードを有効にする（次の行をコメントアウトすると画面が表示される）。
    # options.add_argument('--headless')
    chrome_service = fs.Service(ChromeDriverManager().install())
    driver = webdriver.Chrome(service = chrome_service, options=options)
    driver.get("https://app.highlow.com/quick-demo")
    time.sleep(20)
    print("Initialize")
    driver.find_element(*IndexPageObject.TurboSp).click()
    print("click")
    time.sleep(3)
    driver.find_element(*IndexPageObject.Input).send_keys('8359')
    time.sleep(25)


if __name__ == "__main__":
    browser = Browser()
    print("browser init")

    browser.open_first_page()

    time.sleep(10)

    browser.enable_oneclick()
    time.sleep(1)
    browser.click_element()
