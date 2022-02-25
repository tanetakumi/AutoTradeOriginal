var text = document.evaluate('//*[@id="scroll_panel_1_content"]/div[2]/div/div[1]', document, null, 6, null).snapshotItem(0).innerText;
var mes = text.split(/\n/);
console.log(mes[0]);

var period = mes[3].replace(/[^0-9a-zA-Z\u30a0-\u30ff]/g,'');
if(period.match('スプ')){
    
} else {
    console.log('Non スプレッド');
}


//<non sp>  0 Turbo30s  1  Turbo60s  2  Turbo3m  3  Turbo5m  HighLow15m( 4  sho  5  mid  6  lon )  7  HighLow1h  8  HighLow1d 
//<sp>      9 Turbo30s  10 Turbo60s  11 Turbo3m  12 Turbo5m  HighLow15m( 14 sho  15 mid  16 lon )  17 HighLow1h  18 HighLow1d
console.log(period);
console.log(mes);
if(mes[0] == 'USD/JPY'){

}
console.log(mes[3]);


function confirm(){
    var text = document.evaluate('//*[@id="scroll_panel_1_content"]/div[2]/div/div[1]', document, null, 6, null).snapshotItem(0).innerText;
    var mes = text.split(/\n/);
    return mes[0]+mes[3].replace(/[^0-9a-zA-Z\u30a0-\u30ff]/g,'')=='GBP/JPYHighLowスプレッド30';
} confirm();

"var text = document.evaluate('//*[@id=\"scroll_panel_1_content\"]/div[2]/div/div[1]', document, null, 6, null).snapshotItem(0).innerText;var mes = text.split(/\n/);mes[0]+mes[3].replace(/[^0-9a-zA-Z\u30a0-\u30ff]/g,'')=='';"


var text = document.evaluate('//*[@id="scroll_panel_1_content"]/div[2]/div/div[1]', document, null, 6, null).snapshotItem(0).innerText;var mes = text.split(/\n/);mes[0]+mes[3].replace(/[^0-9a-zA-Z\u30a0-\u30ff]/g,'')=='GBP/JPYHighLowスプレッド30';


"var text = document.evaluate('//*[@id=\"scroll_panel_1_content\"]/div[2]/div/div[1]', document, null, 6, null).snapshotItem(0).innerText;var mes = text.split(/\n/);mes[0]+mes[3].replace(/[^0-9a-zA-Z\u30a0-\u30ff]/g,'')=='GBP/JPYHighLowスプレッド30';"