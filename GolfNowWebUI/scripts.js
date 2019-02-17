//Author : Andrew Millward
function send(){
    console.log("Hello world!");
    var div = document.getElementById("typeMessage");
    var message = new String(div.textContent.trim());
    console.log(message);

    if (message.valueOf() !== "") {
        document.getElementById('typeMessage').innerHTML = "<div id='textBoxDiv' class='s' contenteditable='true' data-ph='Ask away...'></div>"
        var node = document.createElement("div");
        var textnode = document.createTextNode(message);
        var attributenode = document.createAttribute("class");
        attributenode.value = "messageBox messageSent";
        node.appendChild(textnode);
        node.setAttributeNode(attributenode);
        document.getElementById("messagesWrapper").appendChild(node);
        recieve();
    }
}

function recieve() {
    var node = document.createElement("div");
    var textnode = document.createTextNode("Sample reply");
    var attributenode = document.createAttribute("class");
    attributenode.value = "messageBox messageReply";
    node.appendChild(textnode);
    node.setAttributeNode(attributenode);
    document.getElementById("messagesWrapper").appendChild(node);
}