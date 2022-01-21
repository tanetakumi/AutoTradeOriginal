from selenium import webdriver
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome import service as fs
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.options import Options

import time 

options = Options()
options.add_experimental_option('excludeSwitches', ['enable-logging'])
options.use_chromium = True
options.add_argument('--headless')

chrome_service = fs.Service(ChromeDriverManager().install())

driver = webdriver.Chrome(service = chrome_service, options = options)
driver.get("https://app.highlow.com/quick-demo")
time.sleep(60)