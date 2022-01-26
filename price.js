.value = 0;

var ele = document.evaluate('//*[@id="scroll_panel_1_content"]/div[2]/div/div[2]/div/div[1]/div[1]/div[2]/div/input', document, null , 6, null).snapshotItem(0);
ele.value = 0;ele.focus();