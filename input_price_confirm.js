//*[@id="scroll_panel_1_content"]/div[2]/div/div[2]/div/div[1]/div[1]/div[2]/div/input

document.evaluate('//*[@id="scroll_panel_1_content"]/div[2]/div/div[2]/div/div[1]/div[1]/div[2]/div/input', document, null, 6, null).snapshotItem(0).value.replace(/\\|,/g,'');



document.evaluate('//*[@id="scroll_panel_1_content"]/div[2]/div/div[2]/div/div[1]/div[1]/div[2]/div/input', document, null, 6, null).snapshotItem(0).value.replace(/[^0-9]/g,'');