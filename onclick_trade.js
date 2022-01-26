
var data = document.evaluate('//*[@id="scroll_panel_1_content"]/div[2]/div/div[2]/div/div[2]/div[1]/div', document, null, 6, null).snapshotItem(0).getAttribute('data-test');
if(!data.match('Enable')){
    document.evaluate('//*[@id="scroll_panel_1_content"]/div[2]/div/div[2]/div/div[2]/div[2]/div[1]', document, null, 6, null).snapshotItem(0).click();
}

