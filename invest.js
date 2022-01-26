document.getElementById('LOW_TRADE_BUTTON').click()

//
function con(){
    var tabs = document.getElementById('content_1').children;
    for(var i = 0;i<tabs.length;i++){
        if(tabs[i].getAttribute('class').match('active')){
            return tabs[i].children[1].children[1].innerText +'#'+ document.evaluate('//*[@id="root"]/div/div[8]', document, null, 6, null).snapshotItem(0).innerText;
        }
    }    
}con();


"function con(){var tabs = document.getElementById('content_1').children;" +
"for(var i = 0;i<tabs.length;i++){if(tabs[i].getAttribute('class').match('active')){" +
"return tabs[i].children[1].children[1].innerText +'#'+ document.evaluate('//*[@id=\"root\"]/div/div[8]', document, null, 6, null).snapshotItem(0).innerText;}}" +
"}con();"

document.evaluate('/html', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null);
//*[@id="root"]/div/div[8]

document.evaluate('//*[@id="root"]/div/div[8]', document, null, 6, null).snapshotItem(0).click();

//　選択
if(!document.getElementById('ChangingStrikeOOD0').className.match('active')){document.getElementById('ChangingStrikeOOD0').click();}
if(!document.getElementById('60000').className.match('selected')){document.getElementById('60000').click();}
if(!document.getElementById('AUD/JPY').className.match('selected')){document.getElementById('AUD/JPY').click();}


console.log(document.getElementById('content_0').children.length);

//最大の時間をとる
var periods = document.getElementById('content_0').children;
var max = -1, index = -1;
for(var i = 0;i<periods.length;i++){
    var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));
    if(num > 105 & num > max){
        index = i;
        max = num;
    }
}
periods[index].click();


"var periods = document.getElementById('content_0').children;var max = -1, index = -1;for(var i = 0;i<periods.length;i++){var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));if(num > 105 & num > max){index = i;max = num;}}periods[index].click();"

<div data-test="lock_12Px-icon" class="Icon_icon__2UElW "><svg width="12" height="12" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg">
<path fill-rule="evenodd" clip-rule="evenodd" d="M6 1C4.34315 1 3 2.34315 3 4V6C2.44772 6 2 6.44772 2 7V10C2 10.5523 2.44772 11 3 11H9C9.55228 11 10 10.5523 10 10V7C10 6.44772 9.55228 6 9 6V4C9 2.34315 7.65685 1 6 1ZM4 3.8C4 2.80589 4.89543 2 6 2C7.10457 2 8 2.80589 8 3.8V6H4V3.8ZM6 9.5C6.55228 9.5 7 9.05228 7 8.5C7 7.94772 6.55228 7.5 6 7.5C5.44772 7.5 5 7.94772 5 8.5C5 9.05228 5.44772 9.5 6 9.5Z" fill="rgba(255, 102, 77, 1)"></path>
</svg>
</div>