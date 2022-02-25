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

var arr = []


document.getElementById('content_0').children[0].children[2].children[0].getElementsByTagName('svg').getAttribute('class')



//一つの確認
if(document.getElementById('content_0').children.length==1){document.getElementById('content_0').children[0].click();}


if(document.getElementById('content_0').children[2].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')==null){
    console.log("true");   
} else {
   console.log("false");
}
//ロックがかかっていたらnull

var select = 0;
var periods = document.getElementById('content_0').children;
if(select==0){
    for(var i=0;i<periods.length;i++){
        if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){
            var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));
            if(num>5){
                periods[i].click();
                break;
            }
        } 
    }
} else if(select==1){
    periods[1].click()
} else {
    for(var i=periods.length-1;i>=0;i--){
        if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){
            periods[i].click();
            break;
        } 
    }
}

var select = 0;var periods = document.getElementById('content_0').children;
if(select==0){for(var i=0;i<periods.length;i++){if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){
var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));if(num>5){periods[i].click();break;}}}} else if(select==1){periods[1].click()} else {for(var i=periods.length-1;i>=0;i--){
if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){periods[i].click();break;} }}


for(var i;i<periods.length;i++){
    if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){
            var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));
            periods[i].click();
            break;
    }
} 



var max = -1, index = -1;
for(var i = 0;i<periods.length;i++){
    var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));
    if(num > 5 & num > max){
        index = i;
        max = num;
    }
}
periods[index].click();

//*[@id="content_0"]/div[3]/div[3]/span[1]/div

"var periods = document.getElementById('content_0').children;var max = -1, index = -1;for(var i = 0;i<periods.length;i++){var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));if(num > 105 & num > max){index = i;max = num;}}periods[index].click();"


"var select = 0;var periods = document.getElementById('content_0').children;if(select==0){for(var i=0;i<periods.length;i++){if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));if(num>5){periods[i].click();break;}}}} else if(select==1){periods[1].click()} else {for(var i=periods.length-1;i>=0;i--){if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){periods[i].click();break;} }}"