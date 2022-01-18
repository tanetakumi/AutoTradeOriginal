import re

test = "turbo３０秒AUD"

if re.search('(30|３０)(s|秒)', test, flags=re.IGNORECASE):
    print("True")
else:
    print("not include")
