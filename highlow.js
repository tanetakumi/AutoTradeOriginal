
// 要素の座標をクリック
var element = document.getElementById('content_1').children[1].getElementsByTagName('svg')[1];

var cr = element.getBoundingClientRect();

var x = window.pageXOffset + cr.left + cr.width /2;
var y = window.pageYOffset + cr.top +cr.height /2;

var target = document.elementFromPoint(x, y);

target.dispatchEvent(new MouseEvent("click", {
    clientX: x,
    clientY: y
}));



var mouseevent = document.createEvent("MouseEvent");
document.getElementById("hoge").dispatchEvent(mouseevent);



document.evaluate('/html', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null);


var element = document.getElementById('content_1').children[1].getElementsByTagName('svg')[1];
var cr = element.getBoundingClientRect();
(window.pageXOffset + cr.left + cr.width /2).toString() +":"+(window.pageYOffset + cr.top +cr.height /2).toString()



